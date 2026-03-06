using UnityEngine;

public class TutorialAimTarget : MonoBehaviour, ITargetable
{
    [Header("Target Settings")]
    [SerializeField] private Vector3 m_TargetOffset = new(0f, 1f, 0f);

    [Header("Visual Feedback")]
    public MeshMaterialChanger m_MaterialChanger;
    private bool m_IsAimed;

    public bool IsAimed => m_IsAimed;

    public Vector3 GetTargetPosition() => transform.position + m_TargetOffset;

    public Vector3 GetTargetIndicatorPosition() => transform.position + m_TargetOffset;

    public void SetAimedState(bool aimed)
    {
        if (m_IsAimed == aimed) return;
        m_IsAimed = aimed;
    }
}
