using Unity.Cinemachine;
using UnityEngine;

public class TargetGroupManager : MonoBehaviour
{
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private CinemachineTargetGroup m_TargetGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_TargetGroup.AddMember(m_PlayerController.Get().transform, 3f, 1f);
    }
}