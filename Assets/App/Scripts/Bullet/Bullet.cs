using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    float speed;
    int damage;
    float knockback;

    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] private GameObject m_HitPrefab;

    //[Header("Input")]
    //[Header("Output")]

    public Bullet Setup(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;

        StartCoroutine(CheckDistanceFromPlayer());

        return this;
    }
    public Bullet SetKnockback(float knockback)
    {
        this.knockback = knockback;
        return this;
    }

    private void Update()
    {
        rb.position += transform.up * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Bullet hit: " + other.gameObject.name);
        if (!other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Player"))
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (m_HitPrefab)
            {
                GameObject hitVFX = Instantiate(m_HitPrefab, pos, rot);

                Destroy(hitVFX, hitVFX.GetComponent<ParticleSystem>().main.duration);
            }
        }
            if (other.gameObject.TryGetComponent(out EntityTrigger trigger))
        {
            if(knockback > 0) trigger.GetController().GetRigidbody().AddForce(transform.up * knockback);
            trigger.GetController()?.GetHealth().TakeDamage(damage);
        }

        transform.position = Vector3.zero;
        BulletManager.Instance.ReturnBullet(this);
    }

    IEnumerator CheckDistanceFromPlayer()
    {
        yield return new WaitForSeconds(5);
        transform.position = Vector3.zero;
        BulletManager.Instance.ReturnBullet(this);
    }
}