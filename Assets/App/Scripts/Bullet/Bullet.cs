using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    [FormerlySerializedAs("rb")]
    [Header("References")]
    [SerializeField] private Rigidbody m_Rb;

    [SerializeField] private GameObject m_HitPrefab;

    private int m_Damage;
    private float m_Knockback;

    //[Header("Input")]
    //[Header("Output")]

    private Vector3 m_OriginalPosition;

    [Header("Settings")]
    private float m_Speed;

    private void Update()
    {
        m_Rb.position += transform.up * (m_Speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Bullet"))
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (m_HitPrefab && !other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player"))
            {
                GameObject hitVFX = Instantiate(m_HitPrefab, pos, rot);

                Destroy(hitVFX, hitVFX.GetComponent<ParticleSystem>().main.duration);
            }

            if (other.gameObject.TryGetComponent(out EntityTrigger trigger))
            {
                if (m_Knockback > 0) trigger.GetController().GetRigidbody().AddForce(transform.up * m_Knockback);
                trigger.GetController()?.GetHealth().TakeDamage(m_Damage);
            }
        }

        transform.position = Vector3.zero;
        BulletManager.S_Instance.ReturnBullet(this);
    }

    public Vector3 GetShootPosition()
    {
        return m_OriginalPosition;
    }

    public Bullet Setup(int damage, float speed)
    {
        this.m_Damage = damage;
        this.m_Speed = speed;

        m_Rb.linearVelocity = Vector3.zero;
        m_Rb.angularVelocity = Vector3.zero;

        m_OriginalPosition = transform.position;

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
        if (target.TryGetComponent(out IHealth health)) health.TakeDamage(m_Damage);

        transform.position = Vector3.zero;
        BulletManager.S_Instance.ReturnBullet(this);
    }

    private IEnumerator CheckDistanceFromPlayer()
    {
        yield return new WaitForSeconds(5);
        transform.position = Vector3.zero;
        BulletManager.S_Instance.ReturnBullet(this);
    }
}