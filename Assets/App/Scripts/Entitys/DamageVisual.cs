using UnityEngine;

public class DamageVisual : MonoBehaviour
{
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

    public void SetDamage(float value)
    {
        foreach (MeshRenderer renderer in m_Renderers)
        {
            renderer.GetPropertyBlock(m_Block);
            m_Block.SetFloat("_DamageAmount", value);
            renderer.SetPropertyBlock(m_Block);
        }
    }
}