using UnityEngine;

public class DamageVisual : MonoBehaviour
{
    private static readonly int s_DamageAmount = Shader.PropertyToID("_DamageAmount");

    [Header("References")]
    [SerializeField] private MeshRenderer[] m_Renderers;

    [SerializeField] private EntityHealth m_EntityHealth;
    [Range(0f, 1f)] [SerializeField] private float m_DamageAmount;
    private MaterialPropertyBlock m_Block;

    private void Awake()
    {
        m_Block = new MaterialPropertyBlock();
    }

    private void Update()
    {
        SetDamage(1 - m_EntityHealth.GetHealthPercentage());
    }

    private void SetDamage(float value)
    {
        foreach (MeshRenderer rend in m_Renderers)
        {
            rend.GetPropertyBlock(m_Block);
            m_Block.SetFloat(s_DamageAmount, value);
            rend.SetPropertyBlock(m_Block);
        }
    }
}