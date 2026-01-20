using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] int m_Damage;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent(out HurtBox hurtBox))
        {
            hurtBox.TakeDamage(m_Damage);
        }
    }
}