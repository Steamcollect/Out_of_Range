using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    float speed;
    int damage;

    [Header("References")]
    [SerializeField] RSO_PlayerRigidbody playerTransform;
    [SerializeField] Rigidbody rb;

    //[Header("Input")]
    //[Header("Output")]

    public void Setup(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;

        StartCoroutine(CheckDistanceFromPlayer());
    }

    private void Update()
    {
        rb.position += transform.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IHealth health))
        {
            health.TakeDamage(damage);
        }

        if (other.isTrigger) return;

        BulletManager.Instance.ReturnBullet(this);
    }

    IEnumerator CheckDistanceFromPlayer()
    {
        if(Vector3.Distance(playerTransform.Get().position, transform.position) > 50)
        {
            BulletManager.Instance.ReturnBullet(this);
            yield break;
        }

        yield return new WaitForSeconds(5);
        BulletManager.Instance.ReturnBullet(this);
    }
}