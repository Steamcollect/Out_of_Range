using DG.Tweening;
using MVsToolkit.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class VFXController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect visualEffect;

    [Button("Play")]
    public void PlayEvent()
    {
        visualEffect.SendEvent("Dash");  
        Vector3 originPosition = transform.position;
        transform.DOMove(originPosition + Vector3.forward * 10, 0.3f);
        
        this.Delay(() =>
        {
            transform.DOMove(originPosition, 0.3f);
        }, 1f);
    }
}
