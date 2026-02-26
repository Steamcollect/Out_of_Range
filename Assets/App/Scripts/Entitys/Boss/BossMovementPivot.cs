using MVsToolkit.Utilities;
using UnityEngine;

public class BossMovementPivot : RegularSingleton<BossMovementPivot>
{
    [Header("Settings")]
    [SerializeField] float m_CylinderRadius;
    [SerializeField] float m_MaxAngle;

    public Vector3 Pivot
    {
        get
        {
            return transform.position;
        }
        private set { }
    }
    public Vector3 CenterPos
    {
        get
        {
            return transform.position + transform.forward * m_CylinderRadius;
        }
        private set { }
    }

    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public Vector3 ApplyOnCylinder(Vector3 position)
    {
        Vector3 pivot = transform.position;
        float y = position.y;

        Vector3 flat = new Vector3(position.x - pivot.x, 0f, position.z - pivot.z);

        if (flat.sqrMagnitude < 0.0001f)
        {
            Vector3 fallback = transform.forward * m_CylinderRadius;
            return new Vector3(pivot.x + fallback.x, y, pivot.z + fallback.z);
        }

        Vector3 dir = flat.normalized;

        Vector3 projected = pivot + dir * m_CylinderRadius;
        projected.y = y;

        return projected;
    }

    public void IsPosOvertakingMaxAngles(Vector3 pos, ref bool IsOvertakingLeft, ref bool IsOvertakingRight)
    {
        Vector3 pivot = transform.position;

        Vector3 flat = pos - pivot;
        flat.y = 0f;

        IsOvertakingLeft = false;
        IsOvertakingRight = false;

        if (flat.sqrMagnitude < 0.0001f)
            return;

        float angle = Vector3.SignedAngle(transform.forward, flat.normalized, Vector3.up);

        if (angle > m_MaxAngle)
        {
            IsOvertakingRight = true;
        }
        else if (angle < -m_MaxAngle)
        {
            IsOvertakingLeft = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f);
        Gizmos.DrawRay(transform.position, transform.forward * m_CylinderRadius);
        Gizmos.DrawSphere(transform.position + transform.forward * m_CylinderRadius, .5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, -m_MaxAngle, 0f) * transform.forward * m_CylinderRadius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0f, m_MaxAngle, 0f) * transform.forward * m_CylinderRadius);
        MVsGizmos.DrawCircle(transform.position, m_CylinderRadius, Vector3.up);
    }
}