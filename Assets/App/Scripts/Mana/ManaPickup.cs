using System.Collections;
using UnityEngine;

public class ManaPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int m_ManaGiven;

    [SerializeField] float m_StartMoveTime;
    float m_MoveTime;
    [SerializeField] float m_MoveSpeedRemovePerSec;

    [Space(10)]
    [SerializeField] float m_InitAngle;
    [SerializeField] float m_StartPropulsionForce;
    [SerializeField] float m_StartingYVelocity;

    Vector3 m_Velocity;

    [Header("References")]
    [SerializeField] RSO_Mana m_Mana;
    [SerializeField] RSO_PlayerController m_Player;

    PooledObject m_PoolTicket;

    Coroutine m_LifeTimeCor;

    private void Update()
    {
        Vector3 targetPos = Vector3.SmoothDamp(
            transform.position,
            m_Player.Get().GetTargetPosition(),
            ref m_Velocity,
            m_MoveTime);

        if (targetPos.y < m_Player.Get().GetTargetPosition().y) 
            targetPos.y = m_Player.Get().GetTargetPosition().y;
        transform.position = targetPos;

        m_MoveTime = Mathf.Clamp(m_MoveTime - m_MoveSpeedRemovePerSec * Time.deltaTime, 0.01f, m_MoveTime);
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

    public ManaPickup AddForce(Vector3 direction)
    {
        Vector3 dir = (transform.position - m_Player.Get().GetTargetPosition()).normalized;

        float angle = Random.value < .5f ? -m_InitAngle : m_InitAngle;

        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        m_Velocity = rot * dir * m_StartPropulsionForce;
        m_Velocity.y = m_StartingYVelocity;

        m_MoveTime = m_StartMoveTime;

        return this;
    }

    void ReturnToPool()
    {
        if (m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();
        m_PoolTicket.Release();
    }
}