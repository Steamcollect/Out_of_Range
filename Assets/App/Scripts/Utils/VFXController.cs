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

    private static readonly int eventID = Shader.PropertyToID("Shoot");
    private static readonly int trailEnabledID = Shader.PropertyToID("TrailEnabled");

    [Button("Play")]
    public void PlayEvent()
    {
        visualEffect.SendEvent(playEvent);
        visualEffect.SetBool(trailEnabledID, true);
    }

    [Button("Shoot")]
    public void ShootEvent()
    {
        visualEffect.SendEvent(shootEvent);
        visualEffect.SetBool(trailEnabledID, true);
    }
}
