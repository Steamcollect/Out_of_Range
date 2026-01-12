using MVsToolkit.Utils;
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
        visualEffect.SetBool("TrailEnabled", true);
        visualEffect.SendEvent(playEvent);
    }

    [Button("Shoot")]
    public void ShootEvent()
    {
        visualEffect.Reinit();
        visualEffect.SetBool("TrailEnabled", false);
        visualEffect.SendEvent(shootEvent);

        CoroutineUtils.Delay(this, () =>
        {
            PlayEvent();
        }, 3.0f);
    }
}
