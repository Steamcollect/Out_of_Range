using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    float speed;
    int damage;

    [Header("References")]
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
        if(other.TryGetComponent(out EntityTrigger trigger))
        {
            trigger.GetController().GetHealth().TakeDamage(damage);
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