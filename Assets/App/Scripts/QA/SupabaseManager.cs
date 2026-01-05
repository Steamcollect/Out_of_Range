using System;
using UnityEngine;
using Supabase;
using System.Threading.Tasks;

public class SupabaseManager : MonoBehaviour
{
    public static SupabaseManager Instance { get; private set; }
    public bool isInitialized;
    
    // Récupère ces infos dans ton dashboard Supabase : Project Settings > API
    [SerializeField] private string supabaseUrl = "TON_URL_SUPABASE";
    [SerializeField] private string supabaseAnonKey = "TON_ANON_KEY";

    public Client Supabase { get; private set; }

    private async void Awake()
    {
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

    private async Task InitializeSupabase()
    {
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        Supabase = new Client(supabaseUrl, supabaseAnonKey, options);
        await Supabase.InitializeAsync();
        Debug.Log("Supabase initialisé !");
        isInitialized = true;
        RunQueries rq = new RunQueries();
        try
        {
            Guid newRunId = await rq.CreateRun("1.0.0");
            Debug.Log($"Nouveau Run créé avec l'ID : {newRunId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors de la création du Run : {ex.Message}");
        }
    }
}