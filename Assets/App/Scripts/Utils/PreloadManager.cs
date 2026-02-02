using UnityEngine;

public class PreloadManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem[] particlesSystemToPreload;

    private void Start()
    {
        foreach (ParticleSystem ps in particlesSystemToPreload)
        {
            ps.Play();  
        }
    }
}
