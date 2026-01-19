using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] int m_Damage;
    [SerializeField] float m_KnockbackForce;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent(out HurtBox hurtBox))
        {
            hurtBox.TakeDamage(m_Damage);

            if (collision.collider.transform.parent.CompareTag("Player")
                && collision.collider.transform.parent.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(collision.contacts[0].normal * m_KnockbackForce, ForceMode.Impulse);
            }
        }
    }
}