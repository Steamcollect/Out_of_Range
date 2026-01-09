using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Supabase;
using Supabase.Postgrest;

public class DeathVisualizer : MonoBehaviour
{
    [Header("Supabase Config")]
    [SerializeField] private SSO_SupabaseConfig _config;

    [Header("Visualization Settings")]
    [SerializeField] private Color _color = Color.red;
    [Range(0.1f, 5f)] 
    [SerializeField] private float _radius = 0.5f;
    [Range(0f, 1f)] 
    [SerializeField] private float _alpha = 0.5f;
    [SerializeField] private bool _showOnlyWhenSelected = true;

    // Stockage des positions en mémoire (pas sérialisé dans la scène pour ne pas l'alourdir)
    private List<Vector3> _cachedPositions = new List<Vector3>();

    /// <summary>
    /// Bouton accessible via clic-droit sur le composant dans l'inspecteur
    /// </summary>
    [ContextMenu("Fetch All Deaths")]
    public async void FetchDeathData()
    {
        Debug.Log("Récupération des données en cours...");

        // 1. Initialisation d'un client local (nécessaire hors Play Mode)
        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = false,
            AutoConnectRealtime = false
        };
        
        var client = new Supabase.Client(_config.supabaseUrl, _config.supabaseAnonKey, options);
        await client.InitializeAsync();

        // 2. Requête (Sans filtre comme demandé)
        // On récupère uniquement la colonne position
        var result = await client
            .From<DeathModel>()
            .Select("position") 
            .Get();

        // 3. Traitement
        _cachedPositions.Clear();
        
        if (result.Models != null)
        {
            foreach (var death in result.Models)
            {
                // DeathModel.Position est maintenant un PositionDto, on convertit en Vector3
                if (death.Position != null)
                {
                    _cachedPositions.Add(death.Position.ToVector3());
                }
            }
        }

        Debug.Log($"Terminé ! {_cachedPositions.Count} morts récupérées. Regardez la Scene View.");
        
        // Force le rafraichissement de la vue Scène pour afficher les Gizmos immédiatement
#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }

    /// <summary>
    /// Bouton pour effacer la visualisation
    /// </summary>
    [ContextMenu("Clear Data")]
    public void ClearData()
    {
        _cachedPositions.Clear();
#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }

    /// <summary>
    /// C'est ici que la magie opère pour l'affichage dans l'éditeur
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_showOnlyWhenSelected) return;
        DrawGizmosContent();
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showOnlyWhenSelected) return;
        DrawGizmosContent();
    }

    private void DrawGizmosContent()
    {
        if (_cachedPositions == null || _cachedPositions.Count == 0) return;

        // Configuration de la couleur avec l'Alpha réglable
        Gizmos.color = new Color(_color.r, _color.g, _color.b, _alpha);

        foreach (var pos in _cachedPositions)
        {
            Gizmos.DrawSphere(pos, _radius);
        }
    }
}