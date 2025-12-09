using UnityEngine;

public class BlurController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Height = 10.0f;

    [Header("References")]
    [SerializeField] private Material m_FullScreenMaterial;

    [SerializeField] private RSO_PlayerController m_PlayerController;

    private readonly string m_ReferenceName = "_PlayerY";
    private int m_PlayerYid;

    private void Start()
    {
        if (m_FullScreenMaterial != null)
            m_PlayerYid = Shader.PropertyToID(m_ReferenceName);
        else
            Debug.LogError("Attention : Aucun Material assign� au script BlurController !");
    }

    private void Update()
    {
        if (m_FullScreenMaterial && m_PlayerController)
            m_FullScreenMaterial.SetFloat(m_PlayerYid, m_PlayerController.Get().GetTargetPosition().y - m_Height);
    }
}