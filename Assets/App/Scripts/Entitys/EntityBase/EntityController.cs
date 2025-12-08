using System;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityController : MonoBehaviour, ITargetable
{
    [FormerlySerializedAs("targetPos")]
    [Header("Settings")]
    [SerializeField] [Handle(TransformLocationType.Local)]
    private Vector3 m_TargetPos;

    [FormerlySerializedAs("health")]
    [Header("References")]
    [SerializeField] protected EntityHealth m_Health;

    [FormerlySerializedAs("trigger")] [SerializeField] protected EntityTrigger m_Trigger;
    [FormerlySerializedAs("movement")] [SerializeField] protected InterfaceReference<IMovement> m_Movement;
    [FormerlySerializedAs("combat")] [SerializeField] protected EntityCombat m_Combat;


    [FormerlySerializedAs("rb")] [Space(10)] [SerializeField] protected Rigidbody m_Rb;

    //[Header("Input")]
    //[Header("Output")]

    public Action<EntityController> OnDeath;

    protected void Awake()
    {
        m_Trigger.SetController(this);
        m_Health.OnDeath += OnEntityDie;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTargetPosition(), .2f);
    }

    public Vector3 GetTargetPosition()
    {
        return transform.position + m_TargetPos;
    }

    private void OnEntityDie()
    {
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }

    public EntityHealth GetHealth()
    {
        return m_Health;
    }

    public EntityTrigger GetTrigger()
    {
        return m_Trigger;
    }

    public EntityCombat GetCombat()
    {
        return m_Combat;
    }

    public IMovement GetMovement()
    {
        return m_Movement.Value;
    }

    public Rigidbody GetRigidbody()
    {
        return m_Rb;
    }
}