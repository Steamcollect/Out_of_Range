using System;
using Unity.Cinemachine;
using UnityEngine;

public class TargetGroupManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Weight = 3f;
    [SerializeField] private float m_Radius = 1f;
    
    [Header("References")]
    [SerializeField] private CinemachineTargetGroup m_TargetGroup;
    [Space(10)]
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private void Start()
    {
        m_TargetGroup.AddMember(m_PlayerController.Get().transform, m_Weight, m_Radius);
    }

    private void OnEnable()
    {
        m_PlayerController.Get().OnDeath += RemoveTargetFromGroup;
    }
    
    private void OnDisable()
    {
        m_PlayerController.Get().OnDeath -= RemoveTargetFromGroup;
    }
    
    private void RemoveTargetFromGroup(EntityController target)
    {
        m_TargetGroup.RemoveMember(target.transform);
        m_TargetGroup.enabled = false;
    }

    public Vector3 GetTargetPosition() => m_TargetGroup.Transform.position;
    
    public void UpdateTargetGroup() => m_TargetGroup.DoUpdate();
    
    
}