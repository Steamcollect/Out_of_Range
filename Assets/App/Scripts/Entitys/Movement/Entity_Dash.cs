using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Entity_Dash : MonoBehaviour
{
    [FormerlySerializedAs("dashCooldown")]
    [Header("SETTINGS")]
    [SerializeField] private float m_DashCooldown;

    [FormerlySerializedAs("dashForceMode")] [Space(10)] [SerializeField] private ForceMode m_DashForceMode;

    [FormerlySerializedAs("dashDrag")] [Space(10)] [SerializeField] private float m_DashDrag;

    [FormerlySerializedAs("dashForce")] [SerializeField] private float m_DashForce;
    [FormerlySerializedAs("dadhTime")] [SerializeField] private float m_DadhTime;
    [FormerlySerializedAs("invicibilityTime")] [SerializeField] private float m_InvicibilityTime;

    [FormerlySerializedAs("rb")]
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody m_Rb;

    [FormerlySerializedAs("entityHealth")] [SerializeField] private EntityHealth m_EntityHealth;

    private float m_BeginDrag;
    private bool m_CanDash = true;

    public void Dash(Vector3 input)
    {
        if (!m_CanDash) return;

        m_BeginDrag = m_Rb.linearDamping;
        m_Rb.linearDamping = m_DashDrag;
        m_EntityHealth.GainInvincibility(m_InvicibilityTime);

        m_Rb.AddForce(input * m_DashForce, m_DashForceMode);

        StartCoroutine(DashTime());
        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashTime()
    {
        yield return new WaitForSeconds(m_DadhTime);
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