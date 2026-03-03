using UnityEngine;
using DG.Tweening;
using MVsToolkit.Dev;
using UnityEngine.VFX;
using MVsToolkit.Utilities;

public class SpawnVisual : MonoBehaviour
{
    private static readonly int s_CutoffHeightID = Shader.PropertyToID("_CutoffHeight");
    private static readonly int s_VfxHeightID = Shader.PropertyToID("Height");
    private static readonly int s_VfxSpawnDurationID = Shader.PropertyToID("SpawnDuration");
    private static readonly int s_VfxPreSpawnDurationID = Shader.PropertyToID("PreSpawnDuration");

    [Header("Settings")]
    [SerializeField] private float m_Height = 3.0f;
    [SerializeField] private float m_PreSpawnDuration = 0.5f;
    [SerializeField] private float m_SpawnDuration = 1.0f;

    [Header("References")]
    [SerializeField] private VisualEffect m_SpawnVFX;
    [SerializeField] private MeshRenderer[] m_Renderers;

    private MaterialPropertyBlock m_Block;
    private Tween m_SpawnTween;

    private void Awake()
    {
        m_Block = new MaterialPropertyBlock();
    }

    private void Start()
    {
        PlaySpawnVisual();
    }

    private void OnDestroy()
    {
        m_SpawnTween?.Kill();
    }

    [Sirenix.OdinInspector.Button("Play Spawn Visual")]
    public void PlaySpawnVisual()
    {
        m_SpawnVFX.SendEvent("Spawn");
        m_SpawnVFX.SetFloat(s_VfxHeightID, m_Height);
        m_SpawnVFX.SetFloat(s_VfxSpawnDurationID, m_SpawnDuration);
        m_SpawnVFX.SetFloat(s_VfxPreSpawnDurationID, m_PreSpawnDuration);

        float startValue = transform.position.y;
        float endValue = startValue + m_Height;
        float totalDuration = m_PreSpawnDuration + m_SpawnDuration;

        m_SpawnTween?.Kill();

        ApplyHeight(startValue);

        this.Delay(() =>
        {
            m_SpawnTween = DOVirtual.Float(startValue, endValue, m_SpawnDuration, ApplyHeight)
                .SetEase(Ease.Linear)
                .SetLink(gameObject);
        }, m_PreSpawnDuration);

    }

    private void ApplyHeight(float currentHeight)
    {
        if (m_Renderers != null)
        {
            foreach (MeshRenderer rend in m_Renderers)
            {
                if (rend == null) continue;

                rend.GetPropertyBlock(m_Block);
                m_Block.SetFloat(s_CutoffHeightID, currentHeight);
                rend.SetPropertyBlock(m_Block);
            }
        }
    }
}