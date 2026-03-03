using System;
using System.Collections;
using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

public class Entity_Dash : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField] private float m_DashCooldown;

    [Space(10)] 
    [SerializeField] private ForceMode m_DashForceMode;
    [SerializeField] private LayerMask m_PlayerMask;
    [SerializeField] private LayerMask m_DashMask;

    [Space(3)] 
    [SerializeField] LayerMask m_GroundMask;
    [SerializeField] LayerMask m_BorderWallMask;
    [SerializeField] LayerMask m_WallMask;

    [Space(10)] 
    [SerializeField] private float m_DashDrag;
    [SerializeField] float m_MaxDashDrag;

    [Space(3)]
    [SerializeField] private float m_DashForce;
    [SerializeField] float m_MaxDashForce;

    [Space(3)]
    [SerializeField] private float m_DashTime;
    [SerializeField] private float m_InvicibilityTime;

    [Space(3)] 
    [SerializeField] float m_DashCalculationDistance;
    [SerializeField] float m_MaxDashCalculationDistance;

    [Space(10)]
    [SerializeField, ReadOnly] bool m_UseMaxDash = false;

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody m_Rb;
    [SerializeField] PlayerAnimationVisual m_PlayerAnimationVisual;
    [SerializeField] private EntityHealth m_EntityHealth;

    private float m_BeginDrag;
    private bool m_CanDash = true;
    [ReadOnly] public bool IsDashing = false;
    
    public bool CanDash => m_CanDash;

    public Action<float, float> OnDash;
    public Action OnDashInvincibilityEnd;

    public void Dash(Vector3 input)
    {
        if (!m_CanDash) return;

        CalculateDestination(input, out bool disableBorderWall);

        m_BeginDrag = m_Rb.linearDamping;
        m_Rb.linearDamping = (m_UseMaxDash ? m_MaxDashDrag : m_DashDrag);

        m_EntityHealth.GainInvincibility(m_InvicibilityTime);
        this.Delay(() =>
        {
            OnDashInvincibilityEnd?.Invoke();
        }, m_InvicibilityTime + .05f);

        m_Rb.AddForce(input * (m_UseMaxDash ? m_MaxDashForce : m_DashForce), m_DashForceMode);
        
        OnDash?.Invoke(m_DashTime, m_DashCooldown);
        
        StartCoroutine(m_PlayerAnimationVisual.OnDash(m_DashTime));
        StartCoroutine(DashTime(disableBorderWall));
        StartCoroutine(DashCooldown());
    }
    
    void CalculateDestination(Vector3 input, out bool disableBorderWall)
    {
        Vector3 desirePos = transform.position + input * m_DashCalculationDistance;

        MVsDebug.DrawCircle(desirePos, 1, Vector3.up, Color.white, 1);
        disableBorderWall = IsGrounded(desirePos);

        if (!disableBorderWall)
        {
            desirePos = transform.position + input * m_MaxDashCalculationDistance;
            disableBorderWall = IsGrounded(desirePos);
            if (disableBorderWall) m_UseMaxDash = true;
        }

        if (disableBorderWall)
        {
            Vector3 start = transform.position + Vector3.up * 0.5f;
            Vector3 end = desirePos + Vector3.up * 0.5f;

            if (Physics.Linecast(start, end, out RaycastHit hit, m_WallMask))
            {
                Vector3 backedPos = hit.point - input.normalized * 1f; 
                bool groundedAfterBackstep = IsGrounded(backedPos); 
                if (!groundedAfterBackstep) 
                    disableBorderWall = false;

                MVsDebug.DrawCircle(backedPos, 0.5f, Vector3.up, groundedAfterBackstep ? Color.green : Color.red, 1);            }
            }
    }

    public bool IsGrounded(Vector3 position)
    {
        return Physics.Linecast(position + (Vector3.up * .5f), position + (Vector3.down * .5f), m_GroundMask);
    }

    private IEnumerator DashTime(bool disableBorderWall)
    {
        LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_DashMask, true);

        if (disableBorderWall)
        {
            LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_BorderWallMask, true);
        }

        IsDashing = true;
        yield return new WaitForSeconds(m_DashTime);
        IsDashing = false;

        if (disableBorderWall)
        {
            LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_BorderWallMask, false);
        }

        LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_DashMask, false);

        m_Rb.linearVelocity = m_Rb.linearVelocity.normalized;
        m_Rb.linearDamping = m_BeginDrag;

        m_UseMaxDash = false;
    }

    private IEnumerator DashCooldown()
    {
        m_CanDash = false;
        yield return new WaitForSeconds(m_DashCooldown);
        m_CanDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * m_DashCalculationDistance + Vector3.up, 1);
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * m_MaxDashCalculationDistance + Vector3.up, 1);
    }
}