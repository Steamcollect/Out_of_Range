using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
    [Header("Settings")]
    [FoldoutGroup("Left Arm"), SerializeField] float m_RifleAttackShakeLinearForce = .3f;
    [FoldoutGroup("Left Arm"), SerializeField] float m_RifleAttackShakeAngularForce = 30;
    [FoldoutGroup("Left Arm"), SerializeField] float m_RifleAttackShakeDuration = .2f;
    float m_RifleShakeTimer;

    [Space]
    [FoldoutGroup("Left Arm"), SerializeField] float m_RifleStateTransitionDuration = .5f;

    [Space(10)]
    [FoldoutGroup("Right Arm"), SerializeField] float m_GrenadeAttackShakeLinearForce = 3f;
    [FoldoutGroup("Right Arm"), SerializeField] float m_GrenadeAttackShakeAngularForce = 30;
    [FoldoutGroup("Right Arm"), SerializeField] float m_GrenadeAttackShakeDuration = .6f;

    [Space]
    [FoldoutGroup("Right Arm"), SerializeField] float m_GrenadeStateTransitionDuration = .8f;

    Vector3? m_TargetPos;
    bool m_LastTargetNull;

    bool m_IsRightArmShaking = false, m_IsRightArmAiming = false;

    [Header("References")]
    [SerializeField] RSO_PlayerCameraController m_Camera;

    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LArm;
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LArmAttackPivot;
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LArmIdlePivot;
    [Space(5)]
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LAttackPoint;
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LAttackPointPivot;

    [Space(10)]
    [FoldoutGroup("Right Arm"), SerializeField] Transform m_RArm;
    [FoldoutGroup("Right Arm"), SerializeField] Transform m_RArmAttackPivot;
    [FoldoutGroup("Right Arm"), SerializeField] Transform m_RArmIdlePivot;
    [Space(5)]
    [FoldoutGroup("Right Arm"), SerializeField] Transform m_RAttackPoint;
    [FoldoutGroup("Right Arm"), SerializeField] Transform m_RAttackPointPivot;

    //[Header("Input")]
    //[Header("Output")]

    private void LateUpdate()
    {
        if(!m_LastTargetNull)
            m_RifleShakeTimer += Time.deltaTime;

        m_TargetPos = m_Camera.Get().GetTargetHandler().GetTargetPosition();
        bool isTargettingSomething = m_Camera.Get().GetTargetHandler().IsTargetingSomething();

        if (!isTargettingSomething && !m_LastTargetNull && m_RifleShakeTimer >= m_RifleAttackShakeDuration)
        {
            SetRifleToIdle();
        }
        else if(isTargettingSomething && m_LastTargetNull)
        {
            SetRifleToAttack();
        }
    }

    public Transform LeftArmAttack()
    {
        m_RifleShakeTimer = 0;

        m_LArm.localPosition = m_LArmAttackPivot.localPosition;
                
        if (m_TargetPos == null)
            m_LArm.localRotation = m_LArmAttackPivot.localRotation;
        else
            m_LArm.LookAt(m_TargetPos.Value);


        m_LAttackPoint.position = m_LAttackPointPivot.position;
        m_LAttackPoint.rotation = m_LAttackPointPivot.rotation;

        m_LArm.DOKill();
        m_LArm.DOPunchPosition(
            -m_LAttackPoint.forward * m_RifleAttackShakeLinearForce,
            m_RifleAttackShakeDuration,
            20, 1);
        m_LArm.DOPunchRotation(
            m_LAttackPoint.up * m_RifleAttackShakeAngularForce,
            m_RifleAttackShakeDuration,
            20, 1).OnComplete(() =>
            {
                if(m_LastTargetNull) 
                    SetRifleToIdle();
            });

        return m_LAttackPoint;
    }

    void SetRifleToIdle()
    {
        m_LArm.DOKill();
        m_LArm.DOLocalMove(m_LArmIdlePivot.localPosition, m_RifleStateTransitionDuration);
        m_LArm.DOLocalRotate(m_LArmIdlePivot.localEulerAngles, m_RifleStateTransitionDuration);

        m_RifleShakeTimer = 0;
        m_LastTargetNull = true;
    }
    void SetRifleToAttack()
    {
        m_LArm.DOKill();
        m_LArm.DOLocalMove(m_LArmAttackPivot.localPosition, m_RifleStateTransitionDuration);
        m_LArm.DOLocalRotate(m_LArmAttackPivot.localEulerAngles, m_RifleStateTransitionDuration);

        m_LastTargetNull = false;
    }

    public void SetGrenadeAttackPos()
    {
        m_RArm.DOKill();
        m_RArm.DOLocalMove(m_RArmAttackPivot.localPosition, m_GrenadeStateTransitionDuration);
        m_RArm.DOLocalRotate(m_RArmAttackPivot.localEulerAngles, m_GrenadeStateTransitionDuration);

        m_IsRightArmAiming = true;
    }
    public void SetGrenadeIdlePos()
    {
        m_IsRightArmAiming = false;

        if (!m_IsRightArmShaking)
        {
            m_RArm.DOKill();
            m_RArm.DOLocalMove(m_RArmIdlePivot.localPosition, m_GrenadeStateTransitionDuration);
            m_RArm.DOLocalRotate(m_RArmIdlePivot.localEulerAngles, m_GrenadeStateTransitionDuration);
        }
    }

    public Transform RightArmAttack()
    {
        m_IsRightArmShaking = true;
        
        m_RArm.localPosition = m_RArmAttackPivot.localPosition;
        m_RArm.localRotation = m_RArmAttackPivot.localRotation;

        m_RAttackPoint.position = m_RAttackPointPivot.position;
        m_RAttackPoint.rotation = m_RAttackPointPivot.rotation;

        m_RArm.DOKill();
        m_RArm.DOPunchPosition(
            -m_RAttackPoint.forward * m_GrenadeAttackShakeLinearForce,
            m_GrenadeAttackShakeDuration,
            5, .1f);
        m_RArm.DOPunchRotation(
            m_RAttackPoint.up * m_GrenadeAttackShakeAngularForce,
            m_GrenadeAttackShakeDuration,
            5, .1f).OnComplete(() =>
            {
                m_IsRightArmShaking = false;

                if (!m_IsRightArmAiming)
                {
                    m_RArm.DOKill();
                    m_RArm.DOLocalMove(m_RArmIdlePivot.localPosition, m_GrenadeStateTransitionDuration);
                    m_RArm.DOLocalRotate(m_RArmIdlePivot.localEulerAngles, m_GrenadeStateTransitionDuration);
                }
            });

        return m_RAttackPoint;
    }
}