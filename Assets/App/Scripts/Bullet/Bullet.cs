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

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out EntityTrigger trigger))
        {
            if(knockback > 0) trigger.GetController().GetRigidbody().AddForce(transform.up * knockback);
            trigger.GetController()?.GetHealth().TakeDamage(damage);
        }

        if (other.isTrigger) return;

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