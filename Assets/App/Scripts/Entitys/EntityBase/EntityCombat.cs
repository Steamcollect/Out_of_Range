using MVsToolkit.Utils;
using UnityEngine;

public class EntityCombat : MonoBehaviour, ILookAtTarget
{
    [Header("Settings")]
    [SerializeField] float turnSmoothTime;
    Vector3 turnSmoothHozirontalVelocity, turnSmoothVerticalVelocity;

    [Header("References")]
    [SerializeField] Transform verticalPivot;
    [SerializeField] Transform horizontalPivot;

    public void LookAt(Vector3 targetPos)
    {
        Vector3 direction = targetPos - horizontalPivot.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        //
        // 1) ROTATION HORIZONTALE (Y)
        //
        Vector3 horizontalDir = direction;
        horizontalDir.y = 0f; // On ignore la hauteur pour le yaw
        if (horizontalDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotY = Quaternion.LookRotation(horizontalDir);
            horizontalPivot.LookAtSmoothDamp(
                horizontalPivot.position + horizontalDir,
                ref turnSmoothHozirontalVelocity,
                turnSmoothTime
            );
        }

        //
        // 2) ROTATION VERTICALE (X)
        //
        Vector3 verticalDir = direction.normalized;

        // Point vers lequel le pivot vertical doit regarder
        Vector3 verticalLookPoint = verticalPivot.position + verticalDir;

        verticalPivot.LookAtSmoothDamp(
            verticalLookPoint,
            ref turnSmoothVerticalVelocity,
            turnSmoothTime
        );
    }
}
