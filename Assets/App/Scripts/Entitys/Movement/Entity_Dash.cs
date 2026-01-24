using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Entity_Dash : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField] private float m_DashCooldown;

    [Space(10)] 
    [SerializeField] private ForceMode m_DashForceMode;
    [SerializeField] private LayerMask m_PlayerMask;
    [SerializeField] private LayerMask m_DashMask;

    [Space(10)] 
    [SerializeField] private float m_DashDrag;

    [SerializeField] private float m_DashForce;
    [SerializeField] private float m_DashTime;
    [SerializeField] private float m_InvicibilityTime;

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody m_Rb;
    [SerializeField] PlayerAnimationVisual m_PlayerAnimationVisual;
    [SerializeField] private EntityHealth m_EntityHealth;

    private float m_BeginDrag;
    private bool m_CanDash = true;

    public void Dash(Vector3 input)
    {
        if (!m_CanDash) return;

        m_BeginDrag = m_Rb.linearDamping;
        m_Rb.linearDamping = m_DashDrag;
        m_EntityHealth.GainInvincibility(m_InvicibilityTime);

        m_Rb.AddForce(input * m_DashForce, m_DashForceMode);

        StartCoroutine(m_PlayerAnimationVisual.OnDash(m_DashTime));
        StartCoroutine(DashTime());
        StartCoroutine(DashCooldown());
    }
    
    private IEnumerator DashTime()
    {        
        LayerUtils.IgnoreLayerMaskCollision(m_PlayerMask, m_DashMask, true);

        yield return new WaitForSeconds(m_DashTime);
        
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
}