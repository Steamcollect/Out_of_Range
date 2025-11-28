using Sirenix.OdinInspector;
using UnityEngine;

public class DamageVisual : MonoBehaviour
{
    private MeshRenderer[] m_Renderers;
    private MaterialPropertyBlock m_Block;
    [Range(0f, 1f)] [SerializeField] private float m_DamageAmount;

    void Awake()
    {
        m_Renderers = GetComponentsInChildren<MeshRenderer>();

        m_Block = new MaterialPropertyBlock();
    }

    private void Start()
    {
        SetDamage(0);
    }

    private void Update()
    {
        SetDamage(m_DamageAmount);
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