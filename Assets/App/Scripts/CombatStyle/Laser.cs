using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] int m_Damage;

    private void OnTriggerEnter(Collider collid)
    {
        Debug.Log(collid.gameObject.name);
        if(collid.TryGetComponent(out HurtBox hurtBox))
        {
            hurtBox.TakeDamage(m_Damage);
        }
    }
}