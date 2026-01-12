using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class VFXController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private VFXEventAttribute eventAttribute;

    [Header("Settings")]
    [SerializeField] private string playEvent = "OnPlay";
    [SerializeField] private string shootEvent = "Shoot";

    [Button("Play")]
    public void PlayEvent()
    {
        visualEffect.Reinit();
        visualEffect.SendEvent(playEvent);
    }

    [Button("Shoot")]
    public void ShootEvent()
    {
        visualEffect.Reinit();
        visualEffect.SendEvent(shootEvent);
    }
}
