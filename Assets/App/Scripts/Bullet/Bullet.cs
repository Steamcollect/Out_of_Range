using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_Speed;
    [SerializeField] int m_Damage;

    Vector3 m_OriginalPosition;

    [Header("References")]
    [SerializeField] Rigidbody m_RigidBody;
    [SerializeField] GameObject m_HitPrefab;

    Coroutine m_TimeBeforeResetCoroutine;

    [Header("Output")]
    [SerializeField] UnityEvent m_OnImpact;
    PooledObject m_PoolTicket;
    
    public Bullet Setup()
    {
        m_RigidBody.linearVelocity = Vector3.zero;
        m_RigidBody.angularVelocity = Vector3.zero;
        
        m_RigidBody.linearVelocity = transform.up * m_Speed;
        
        m_OriginalPosition = transform.position;

        if (m_TimeBeforeResetCoroutine != null) StopCoroutine(m_TimeBeforeResetCoroutine);
        m_TimeBeforeResetCoroutine = StartCoroutine(TimeBeforeReset());

        return this;
    }

    public Bullet SetDamage(int damage)
    {
        m_Damage = damage;
        return this;
    }
    public Bullet SetSpeed(float speed)
    {
        m_Speed = speed;
        return this;
    }

    public void Impact(GameObject target)
    {
        if (target.TryGetComponent(out IHealth health))
        {
            health.TakeDamage(m_Damage);
        }
        m_OnImpact.Invoke();

        ReleaseBullet();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        ContactPoint contact = other.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (other.gameObject.TryGetComponent(out HurtBox hurtBox))
            hurtBox.TakeDamage(m_Damage);
        else
            PoolManager.Instance.Spawn(m_HitPrefab, pos, rot);

        m_OnImpact.Invoke();
        ReleaseBullet();
    }

    IEnumerator TimeBeforeReset()
    {
        yield return new WaitForSeconds(5);
        ReleaseBullet();
    }

    private void ReleaseBullet()
    {
        if(m_TimeBeforeResetCoroutine != null) StopCoroutine(m_TimeBeforeResetCoroutine);

        if (m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();
        m_PoolTicket.Release();
    }

    public Vector3 GetShootPosition() => m_OriginalPosition;
}