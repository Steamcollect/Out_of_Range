using System;
using UnityEngine;
using Supabase;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

public class SupabaseManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private RSE_OnPlayerDie m_OnPlayerDie;
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private RSE_OnCheckpointRegistered m_OnCheckpointRegistered;
    
    [Header("Supabase Config")]
    [SerializeField] private SSO_SupabaseConfig m_SupabaseConfig;
    
    public static SupabaseManager Instance { get; private set; }
    private bool m_IsInitialized;
    public bool IsInitialized => m_IsInitialized;
    public Client Supabase { get; private set; }
    private Guid runID = Guid.Empty;
    
    private async void Awake()
    {
        // On enregistre pas de données hors des builds
        if (Application.isEditor)
        {
            return;
        }
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Connexion à Supabase
            bool initOk = await InitializeSupabase();
            if (!initOk)
            {
                Debug.LogError("SupabaseManager : initialisation échouée dans Awake.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (Application.isEditor)
        {
            return;
        }
        
        m_OnPlayerDie.Action += HandlePlayerDeath;
        m_OnCheckpointRegistered.Action += HandleCheckpoint;
    }

    private void OnDisable()
    {
        m_OnPlayerDie.Action -= HandlePlayerDeath;
        m_OnCheckpointRegistered.Action -= HandleCheckpoint;
    }
    
    private async void HandlePlayerDeath()
    {
        DeathQueries dq = new DeathQueries();
        await dq.CreateDeath(runID, m_PlayerController.Get().transform.position);
    }
    
    private async void HandleCheckpoint()
    {
        CheckpointQueries cq = new CheckpointQueries();
        await cq.CreateCheckpoint(runID);
    }
    
    private async Task<bool> InitializeSupabase()
    {
        if (m_SupabaseConfig.supabaseUrl == "YOUR_SUPABASE_URL" || m_SupabaseConfig.supabaseAnonKey == "YOUR_SUPABASE_ANON_KEY")
        {
            Debug.LogError("SupabaseManager : URL ou clé anonyme Supabase non configurée.");
            m_IsInitialized = false;
            return false;
        }
        
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        Supabase = new Client(m_SupabaseConfig.supabaseUrl, m_SupabaseConfig.supabaseAnonKey, options);

        try
        {
            // Initialise le client Supabase
            await Supabase.InitializeAsync();

            // Vérification supplémentaire : tenter un appel HTTP léger vers l'API REST
            // Pour beaucoup de projets Supabase, /rest/v1/ renverra 404 si aucune table n'est faite de la requête,
            // mais une réponse (200..499) indique que le service est joignable.
            bool reachable = false;
            using (var http = new HttpClient())
            {
                http.Timeout = TimeSpan.FromSeconds(5);
                // Fournir la clé anonyme dans l'en-tête pour représenter la même autorisation que le client
                http.DefaultRequestHeaders.Add("apikey", m_SupabaseConfig.supabaseAnonKey);
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", m_SupabaseConfig.supabaseAnonKey);

                var healthUrl = m_SupabaseConfig.supabaseUrl?.TrimEnd('/') + "/rest/v1/";
                try
                {
                    var resp = await http.GetAsync(healthUrl);
                    // On considère que toute réponse HTTP (200-499) signifie que le serveur a répondu correctement
                    if (resp != null && (int)resp.StatusCode >= 200 && (int)resp.StatusCode < 500)
                        reachable = true;
                    else
                        Debug.LogWarning($"Supabase health check returned status {(resp != null ? ((int)resp.StatusCode).ToString() : "no response")}.");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Supabase health check request failed: {e.Message}");
                }
            }

            if (!reachable)
            {
                m_IsInitialized = false;
                Debug.LogError("Supabase initialisation échouée : l'URL Supabase n'est pas joignable.");
                return false;
            }

            m_IsInitialized = true;
            Debug.Log("Supabase initialisé et joignable.");

            RunQueries rq = new RunQueries();
            String version = Application.version;
            runID = await rq.CreateRun(version);

            // Si runID est renseigné, on considère l'initialisation pleinement réussie
            if (runID != Guid.Empty)
            {
                Debug.Log($"Run créé avec ID: {runID}");
                return true;
            }

            Debug.LogWarning("Supabase initialisé mais la création de run a retourné un GUID vide.");
            return true; // On considère que la connexion est ok même si la création du run a retourné vide
        }
        catch (Exception ex)
        {
            m_IsInitialized = false;
            Debug.LogError($"Erreur durant l'initialisation de Supabase : {ex}");
            return false;
        }
    }
}