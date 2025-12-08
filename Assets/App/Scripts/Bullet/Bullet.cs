using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Speed;
    [SerializeField] private int m_Damage;
    [SerializeField] private float m_Knockback;

    [Header("References")]
    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private GameObject m_HitPrefab;

    private Vector3 m_OriginalPosition;

    private PooledObject m_PoolTicket;
    
    public Vector3 GetShootPosition() => m_OriginalPosition;
    
    public Bullet Setup(int damage, float speed)
    {
        this.m_Damage = damage;
        this.m_Speed = speed;

        m_RigidBody.linearVelocity = Vector3.zero;
        m_RigidBody.angularVelocity = Vector3.zero;
        
        m_OriginalPosition = transform.position;

        StartCoroutine(CheckDistanceFromPlayer());

        return this;
    }
    public Bullet SetKnockback(float knockback)
    {
        this.m_Knockback = knockback;
        return this;
    }

    private void Update()
    {
        m_RigidBody.position += transform.up * (m_Speed * Time.deltaTime);
    }
    
    public void Impact(GameObject target)
    {
        if (target.TryGetComponent(out IHealth health))
        {
            health.TakeDamage(m_Damage);
        }
        
        transform.position = Vector3.zero;
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

            if (other.gameObject.TryGetComponent(out EntityTrigger trigger))
            {
                if (m_Knockback > 0)
                {
                    trigger.GetController().GetRigidbody().AddForce(transform.up * m_Knockback);
                }
                trigger.GetController()?.GetHealth().TakeDamage(m_Damage);
            }
        }

        transform.position = Vector3.zero;
        ReleaseBullet();
    }

    IEnumerator CheckDistanceFromPlayer()
    {
        yield return new WaitForSeconds(5);
        transform.position = Vector3.zero;
        ReleaseBullet();
    }

    private void ReleaseBullet()
    {
        if(m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();

        m_PoolTicket.Release();
    }
}