using System.Collections;
using MVsToolkit.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerAnimationVisual : MonoBehaviour
{
    [FoldoutGroup("Idle Anim"), SerializeField] float m_Speed;
    [FoldoutGroup("Idle Anim"), SerializeField] float m_Amplitude;

    [FoldoutGroup("Run Anim"), SerializeField] float m_HeadMovementDistOffset;
    [FoldoutGroup("Run Anim"), SerializeField] float m_HeadMovementTime;

    [FoldoutGroup("Run Anim"), SerializeField] float m_ArmsMovementTime;

    Vector3 m_HeadMovementVelocity;
    Vector3 m_ArmsMovementVelocity;

    [FoldoutGroup("Look At Rotation"), SerializeField] float m_ArmsRotationTime;
    [FoldoutGroup("Look At Rotation"), SerializeField] float m_HeadRotationTime;

    Vector3 m_ArmsRotationVelocity;
    Vector3 m_HeadRotationVelocity;

    [Space(10)]
    [FoldoutGroup("Dash Anim"), SerializeField] GameObject[] m_PlayerMeshs;
    [FoldoutGroup("Dash Anim"), SerializeField] private VisualEffect m_DashEffect;

    [Header("References")]
    [SerializeField] Transform m_ArmsPivot;
    [SerializeField] Transform m_HeadPivot;

    private void Start()
    {
        m_ArmsPivot.SetParent(null);
    }

    private void Update()
    {
        m_HeadPivot.localPosition += (Vector3.up * Mathf.Sin(Time.time * m_Speed) * m_Amplitude);

        m_ArmsPivot.position = Vector3.SmoothDamp(
    m_ArmsPivot.position,
    transform.position,
    ref m_ArmsMovementVelocity,
    m_ArmsMovementTime);
    }

    public void OnMove(Vector3 direction)
    {
        m_HeadPivot.localPosition = Vector3.SmoothDamp(
            m_HeadPivot.localPosition,
            direction * m_HeadMovementDistOffset,
            ref m_HeadMovementVelocity,
            m_HeadMovementTime);
    }

    public void RotateToward(Vector3 target)
    {
        target.y = m_ArmsPivot.position.y;
        m_ArmsPivot.LookAtSmoothDamp(target, ref m_ArmsRotationVelocity, m_ArmsRotationTime);
        m_HeadPivot.LookAtSmoothDamp(target, ref m_HeadRotationVelocity, m_HeadRotationTime);
    }

    public IEnumerator OnDash(float dashTime)
    {
        foreach (var mesh in m_PlayerMeshs)
            mesh.SetActive(false);
        
        m_DashEffect.SendEvent("Dash");

        yield return new WaitForSeconds(dashTime);

        foreach (var mesh in m_PlayerMeshs)
            mesh.SetActive(true);
    }
}