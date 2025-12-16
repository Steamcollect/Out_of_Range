using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Entity_Dash : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField] private float m_DashCooldown;

    [Space(10)] 
    [SerializeField] private ForceMode m_DashForceMode;

    [Space(10)] 
    [SerializeField] private float m_DashDrag;

    [SerializeField] private float m_DashForce;
    [SerializeField] private float m_DashTime;
    [SerializeField] private float m_InvicibilityTime;

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody m_Rb;

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

        StartCoroutine(DashTime());
        StartCoroutine(DashCooldown());
    }
    
    private IEnumerator DashTime()
    {
        Physics.IgnoreLayerCollision(8, 9, true);
        Physics.IgnoreLayerCollision(8, 6, true);
        Physics.IgnoreLayerCollision(12, 6, true);

        Debug.Log("Dash started");
        yield return new WaitForSeconds(m_DashTime);
     
        Physics.IgnoreLayerCollision(8, 9, false);
        Physics.IgnoreLayerCollision(8, 6, false);
        Physics.IgnoreLayerCollision(12, 6, false);
        Debug.Log("Dash ended");
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