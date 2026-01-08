using System;
using System.Threading;
using UnityEngine;
using Supabase;
using System.Threading.Tasks;

public class SupabaseManager : MonoBehaviour
{
    public RSE_OnPlayerDie m_OnPlayerDie;
    public RSO_PlayerController m_PlayerController;
    public RSE_OnCheckpointRegistered m_OnCheckpointRegistered;
    
    public static SupabaseManager Instance { get; private set; }
    private bool m_IsInitialized;
    public bool IsInitialized => m_IsInitialized;
    
    [SerializeField] private SSO_SupabaseConfig m_SupabaseConfig;

    public Client Supabase { get; private set; }
    
    private Guid runID = Guid.Empty;
    
    private async void Awake()
    {
        // On enregistre pas de données hors des builds
        if (Application.isEditor)
        {
            //return;
        }
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await InitializeSupabase();
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
            //return;
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
    
    private async Task InitializeSupabase()
    {
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        Supabase = new Client(m_SupabaseConfig.supabaseUrl, m_SupabaseConfig.supabaseAnonKey, options);
        await Supabase.InitializeAsync();
        m_IsInitialized = true;
        RunQueries rq = new RunQueries();
        String version = Application.version;
        runID = await rq.CreateRun(version);
    }
}