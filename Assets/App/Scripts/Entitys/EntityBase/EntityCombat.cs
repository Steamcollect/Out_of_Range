using MVsToolkit.Utils;
using UnityEngine;

public class EntityCombat : MonoBehaviour, ILookAtTarget
{
    [Header("Settings")]
    [SerializeField] float turnSmoothTime;
    Vector3 turnSmoothHozirontalVelocity, turnSmoothVerticalVelocity;

    [Header("References")]
    [SerializeField] protected CombatStyle currentCombatStyle;

    [Space(10)]
    [SerializeField] protected Transform verticalPivot;
    [SerializeField] protected Transform horizontalPivot;

    public virtual void Attack()
    {
        currentCombatStyle.Attack();
    }

    public virtual void LookAt(Vector3 targetPos)
    {
        Vector3 direction = targetPos - horizontalPivot.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        if (horizontalPivot)
        {
            Vector3 horizontalDir = direction;
            horizontalDir.y = 0f;
            if (horizontalDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotY = Quaternion.LookRotation(horizontalDir);
                horizontalPivot.LookAtSmoothDamp(
                    horizontalPivot.position + horizontalDir,
                    ref turnSmoothHozirontalVelocity,
                    turnSmoothTime
                );
            }
        }

        if (verticalPivot)
        {
            Vector3 verticalDir = direction.normalized;

            Vector3 verticalLookPoint = verticalPivot.position + verticalDir;

            verticalPivot.LookAtSmoothDamp(
                verticalLookPoint,
                ref turnSmoothVerticalVelocity,
                turnSmoothTime
            );
        }
    }

    public CombatStyle GetCombatStyle() => currentCombatStyle;
}