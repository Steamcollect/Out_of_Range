using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ManaPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int m_ManaGiven;

    [Space(10)]
    [SerializeField] float m_InitVelocity;
    [SerializeField] float m_InitAngle = 30;
    [SerializeField] float m_MoveTime;
    [SerializeField] float m_RangeToPickUp;
    [SerializeField] float m_MoveSpeedRemovePerSec;
    [SerializeField] float m_TimeBeforeCanBePick;

    Vector3 m_Velocity;
    bool m_CanBePick = false;
    bool m_IsPickingUp = false;

    [Header("References")]
    [SerializeField] RSO_Mana m_Mana;
    [SerializeField] RSO_PlayerController m_Player;

    [Space(10)]
    [SerializeField] Rigidbody m_Rb;
    [SerializeField] Collider m_Collid;

    PooledObject m_PoolTicket;

    Coroutine m_LifeTimeCor;

    private void Update()
    {
        if (m_CanBePick && m_IsPickingUp)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                m_Player.Get().GetTargetPosition(),
                ref m_Velocity,
                m_MoveTime);
            
            m_MoveTime = Mathf.Clamp(m_MoveTime - m_MoveSpeedRemovePerSec * Time.deltaTime, 0, m_MoveTime);
        }
    }

    private void FixedUpdate()
    {
        if(m_CanBePick && !m_IsPickingUp && Vector3.Distance(m_Player.Get().GetTargetPosition(), transform.position) <= m_RangeToPickUp)
        {
            m_Collid.isTrigger = true;
            m_Rb.isKinematic = true;

            m_IsPickingUp = true;


            Vector3 dir = (transform.position - m_Player.Get().GetTargetPosition()).normalized;

            float angle = Random.value < .5f ? -m_InitAngle : m_InitAngle;

            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            m_Velocity = rot * dir * m_InitVelocity;
        }
    }

    public void Setup()
    {
        m_LifeTimeCor = StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(20);
        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_Mana.Add(m_ManaGiven);

            if(m_LifeTimeCor != null) StopCoroutine(m_LifeTimeCor);
            ReturnToPool();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_RangeToPickUp);
    }

    public void AddForce(Vector3 direction, ForceMode force)
    {
        m_Rb.AddForce(direction, force);
    }

    public IEnumerator SpawningCooldown()
    {
        m_CanBePick = false;
        yield return new WaitForSeconds(m_TimeBeforeCanBePick);
        m_CanBePick = true;
    }

    void ReturnToPool()
    {
        if (m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();
        m_PoolTicket.Release();
    }
}