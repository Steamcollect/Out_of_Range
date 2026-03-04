using UnityEngine;

public class TutorialAimTarget : MonoBehaviour, ITargetable
{
    #region Serialized Fields

    [Header("Target Settings")]
    [SerializeField] private Vector3 m_TargetOffset = new(0f, 1f, 0f);

    [Header("Visual Feedback")]
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private Color m_DefaultColor = Color.white;
    [SerializeField] private Color m_AimedColor = Color.yellow;


    #endregion

    #region Private Fields

    private bool m_IsAimed;
    private MaterialPropertyBlock m_PropBlock;

    #endregion

    #region Properties

    public bool IsAimed => m_IsAimed;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        m_PropBlock = new MaterialPropertyBlock();
        SetColor(m_DefaultColor);
    }

    #endregion

    #region ITargetable

    public Vector3 GetTargetPosition() => transform.position + m_TargetOffset;

    public Vector3 GetTargetIndicatorPosition() => transform.position + m_TargetOffset;

    #endregion

    #region Public API

    /// <summary>
    /// Called by TutorialAimSequence to notify this dummy it is currently aimed at.
    /// </summary>
    public void SetAimedState(bool aimed)
    {
        if (m_IsAimed == aimed) return;
        m_IsAimed = aimed;
        SetColor(aimed ? m_AimedColor : m_DefaultColor);
    }

    #endregion

    #region Helpers

    private void SetColor(Color color)
    {
        if (m_Renderer == null) return;
        m_Renderer.GetPropertyBlock(m_PropBlock);
        m_PropBlock.SetColor("_BaseColor", color); // URP
        m_PropBlock.SetColor("_Color", color);     // Built-in
        m_Renderer.SetPropertyBlock(m_PropBlock);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + m_TargetOffset, 0.25f);
    }

    public void Debugger()
    {
        Debug.Log($"IsAimed: {m_IsAimed}");
    }
    #endregion
}
