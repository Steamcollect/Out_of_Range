using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class VFXController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect visualEffect;

    [Header("Settings")]
    [SerializeField] private string playEvent = "OnPlay";
    [SerializeField] private string shootEvent = "Shoot";

    [Button("Play")]
    public void PlayEvent()
    {
        Debug.Log("Play");
        visualEffect.Reinit();
        visualEffect.SendEvent(playEvent);
    }

    [Button("Shoot")]
    public void ShootEvent()
    {
        Debug.Log("Shoot");
        visualEffect.Reinit();
        visualEffect.SendEvent(shootEvent);
    }
}
