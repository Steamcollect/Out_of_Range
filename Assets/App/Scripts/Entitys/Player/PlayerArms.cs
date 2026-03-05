using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_AttackShakeLinearForce;
    [SerializeField] float m_AttackShakeAngularForce;
    [SerializeField] float m_AttackShakeDuration;
    float m_ShakeTimer;

    [Space]
    [SerializeField] float m_StateTransitionDuration;

    Vector3? m_TargetPos;
    bool m_LastTargetNull;

    [Header("References")]
    [SerializeField] RSO_PlayerCameraController m_Camera;

    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LArm;
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LArmAttackPivot;
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LArmIdlePivot;
    [Space(5)]
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LAttackPoint;
    [FoldoutGroup("Left Arm"), SerializeField] Transform m_LAttackPointPivot;

    //[Header("Input")]
    //[Header("Output")]

    private void LateUpdate()
    {
        if(!m_LastTargetNull)
            m_ShakeTimer += Time.deltaTime;

        m_TargetPos = m_Camera.Get().GetTargetHandler().GetTargetPosition();
        bool isTargettingSomething = m_Camera.Get().GetTargetHandler().IsTargetingSomething();

        if (!isTargettingSomething && !m_LastTargetNull && m_ShakeTimer >= m_AttackShakeDuration)
        {
            SetToIdle();
        }
        else if(isTargettingSomething && m_LastTargetNull)
        {
            SetToAttack();
        }
    }

    public Transform LeftArmAttack()
    {
        m_ShakeTimer = 0;

        m_LArm.localPosition = m_LArmAttackPivot.localPosition;
                
        if (m_TargetPos == null)
            m_LArm.localRotation = m_LArmAttackPivot.localRotation;
        else
            m_LArm.LookAt(m_TargetPos.Value);


        m_LAttackPoint.position = m_LAttackPointPivot.position;
        m_LAttackPoint.rotation = m_LAttackPointPivot.rotation;

        m_LArm.DOKill();
        m_LArm.DOPunchPosition(
            -m_LAttackPoint.forward * m_AttackShakeLinearForce,
            m_AttackShakeDuration,
            20, 1);
        m_LArm.DOPunchRotation(
            Vector3.up * m_AttackShakeLinearForce,
            m_AttackShakeDuration,
            20, 1).OnComplete(() =>
            {
                if(m_LastTargetNull) 
                    SetToIdle();
            });

        return m_LAttackPoint;
    }

    void SetToIdle()
    {
        m_LArm.DOKill();
        m_LArm.DOLocalMove(m_LArmIdlePivot.localPosition, m_StateTransitionDuration);
        m_LArm.DOLocalRotate(m_LArmIdlePivot.localEulerAngles, m_StateTransitionDuration);

        m_ShakeTimer = 0;
        m_LastTargetNull = true;
    }
    void SetToAttack()
    {
        m_LArm.DOKill();
        m_LArm.DOLocalMove(m_LArmAttackPivot.localPosition, m_StateTransitionDuration);
        m_LArm.DOLocalRotate(m_LArmAttackPivot.localEulerAngles, m_StateTransitionDuration);

        m_LastTargetNull = false;
    }
}