using UnityEngine;
using MVsToolkit.Dev;
using System;

public abstract class EntityController : MonoBehaviour, ITargetable
{
    [Header("Settings")]
    [SerializeField] Vector3 m_TargetPos;

    [Header("References")]
    [SerializeField] protected EntityHealth m_Health;
    [SerializeField] protected InterfaceReference<IMovement> m_Movement;
    [SerializeField] protected EntityCombat m_Combat;
        
    [Space(10)]
    [SerializeField] protected Rigidbody m_Rb;

    //[Header("Input")]
    //[Header("Output")]

    public Action<EntityController> OnDeath;

    protected virtual void Awake()
    {
        m_Health.OnDeath += OnEntityDie;
    }

    protected virtual void OnEntityDie()
    {
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }

    public Vector3 GetTargetPosition()
    {
        return transform.position + m_TargetPos;
    }

    public EntityHealth GetHealth() {  return m_Health; }
    public EntityCombat GetCombat() { return m_Combat; }
    public IMovement GetMovement() { return m_Movement.Value; }

    public Rigidbody GetRigidbody() { return m_Rb; }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + GetTargetPosition(), .2f);
    }
}