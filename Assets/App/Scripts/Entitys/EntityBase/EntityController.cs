using UnityEngine;
using MVsToolkit.Dev;

public class EntityController : MonoBehaviour, ITargetable
{
    [Header("Settings")]
    [SerializeField, Handle(TransformLocationType.Local)] Vector3 targetPos;

    [Header("References")]
    [SerializeField] protected EntityHealth health;
    [SerializeField] protected EntityTrigger trigger;
    [SerializeField] protected InterfaceReference<IMovement> movement;
    [SerializeField] protected EntityCombat combat;

    [Space(10)]
    [SerializeField] protected Rigidbody rb;

    //[Header("Input")]
    //[Header("Output")]

    public Vector3 GetTargetPosition()
    {
        return transform.position + targetPos;
    }

    public EntityHealth GetHealth() {  return health; }
    public EntityTrigger GetTrigger() { return trigger; }
    public EntityCombat GetCombat() { return combat; }
    public IMovement GetMovement() { return movement.Value; }

    public Rigidbody GetRigidbody() { return rb; }
}