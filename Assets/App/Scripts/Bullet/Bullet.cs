using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Speed;
    [SerializeField] private int m_Damage;
    [SerializeField] private float m_Knockback;

    [Header("References")]
    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private GameObject m_HitPrefab;

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnImpact;
    
    private Vector3 m_OriginalPosition;

    private PooledObject m_PoolTicket;
    
    public Vector3 GetShootPosition() => m_OriginalPosition;

    public Bullet Setup()
    {
        m_RigidBody.linearVelocity = Vector3.zero;
        m_RigidBody.angularVelocity = Vector3.zero;
        
        m_RigidBody.linearVelocity = transform.up * m_Speed;

        StartCoroutine(CheckDistanceFromPlayer());

        return this;
    }
    public Bullet SetKnockback(float knockback)
    {
        this.m_Knockback = knockback;
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
        if (!other.gameObject.CompareTag("Bullet"))
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (m_HitPrefab && (!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player")))
            {
                GameObject hitVFX = Instantiate(m_HitPrefab, pos, rot);

                Destroy(hitVFX, hitVFX.GetComponent<ParticleSystem>().main.duration);
            }

            if(other.gameObject.TryGetComponent(out EntityController controller))
                controller.GetRigidbody().AddForce(transform.up * m_Knockback);

            if (other.gameObject.TryGetComponentInChildrens(out IHealth health))
                health.TakeDamage(m_Damage);
            
            m_OnImpact.Invoke();
        }

        ReleaseBullet();
    }

    IEnumerator CheckDistanceFromPlayer()
    {
        yield return new WaitForSeconds(5);
        ReleaseBullet();
    }

    private void ReleaseBullet()
    {
        if(m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();
        m_PoolTicket.Release();
    }
}