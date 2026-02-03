using System;
using System.Collections;
using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using UnityEngine;

public class Entity_Dash : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField] private float m_DashCooldown;

    [Space(10)] 
    [SerializeField] private ForceMode m_DashForceMode;
    [SerializeField] private LayerMask m_PlayerMask;
    [SerializeField] private LayerMask m_DashMask;

    [Space(5)] 
    [SerializeField] LayerMask m_GroundMask;
    [SerializeField] LayerMask m_BorderWallMask;
    [SerializeField] LayerMask m_WallMask;

    [Space(10)] 
    [SerializeField] private float m_DashDrag;

    [SerializeField] private float m_DashForce;
    [SerializeField] private float m_DashTime;
    [SerializeField] private float m_InvicibilityTime;

    [Space(10)] 
    [SerializeField] float dashCalculationDistance;

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody m_Rb;
    [SerializeField] PlayerAnimationVisual m_PlayerAnimationVisual;
    [SerializeField] private EntityHealth m_EntityHealth;

    private float m_BeginDrag;
    private bool m_CanDash = true;
    [ReadOnly] public bool IsDashing = false;

    public Action<float, float> OnDash;

    public void Dash(Vector3 input)
    {
        if (!m_CanDash) return;

        CalculateDestination(input, out bool disableBorderWall);

        m_BeginDrag = m_Rb.linearDamping;
        m_Rb.linearDamping = m_DashDrag;
        m_EntityHealth.GainInvincibility(m_InvicibilityTime);

        m_Rb.AddForce(input * m_DashForce, m_DashForceMode);
        
        OnDash?.Invoke(m_DashTime, m_DashCooldown);

        StartCoroutine(m_PlayerAnimationVisual.OnDash(m_DashTime));
        StartCoroutine(DashTime(disableBorderWall));
        StartCoroutine(DashCooldown());
    }
    
    void CalculateDestination(Vector3 input, out bool disableBorderWall)
    {
        Vector3 desirePos = transform.position + input * dashCalculationDistance;

        MVsDebug.DrawCircle(desirePos, 1, Vector3.up, Color.white, 1);
        disableBorderWall = IsGrounded(desirePos);

        if (disableBorderWall)
        {
            if(Physics.Linecast(transform.position + Vector3.up * .5f, desirePos + Vector3.up * .5f, m_WallMask))
            {

            }
        }
    }

    bool IsGrounded(Vector3 position) => Physics.Linecast(position + (Vector3.up * .5f), position + (Vector3.down * .5f), m_GroundMask);

    private IEnumerator DashTime(bool disableBorderWall)
    {
        if (disableBorderWall)
        {
            LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_BorderWallMask, true);
            m_Rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }

        IsDashing = true;
        yield return new WaitForSeconds(m_DashTime);
        IsDashing = false;

        if (disableBorderWall)
        {
            LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_BorderWallMask, false);
            m_Rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_DashMask, false);

        m_Rb.linearVelocity = m_Rb.linearVelocity.normalized;
        m_Rb.linearDamping = m_BeginDrag;
    }

    private IEnumerator DashCooldown()
    {
        m_CanDash = false;
        yield return new WaitForSeconds(m_DashCooldown);
        m_CanDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * dashCalculationDistance + Vector3.up, 1);
        Gizmos.DrawRay(transform.position + Vector3.forward * dashCalculationDistance + Vector3.up * .5f, Vector3.down);
    }
}