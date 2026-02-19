using MVsToolkit.Utilities;
using UnityEngine;

public class BossMovementController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_MoveSpeed;

    [Space(10)]
    [SerializeField, Tooltip("Center of the rotation, the body will move around it")] Vector3 m_PivotPos;
    [SerializeField] float m_Distance;
    [SerializeField] float m_VerticalSpeedRatio = .2f;

    [Space(10)]
    [SerializeField, Range(0, 360)] int m_MaxHorizontalAngle;
    [SerializeField] int m_AngleOffset;

    float m_PosY;
    public Vector2 m_Input;

    //[Header("References")]
    Transform m_RoomPivotPoint;
    Transform m_BodyPivotPoint;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        m_RoomPivotPoint = new GameObject("RoomPivotPoint").transform;
        m_RoomPivotPoint.position = m_PivotPos;

        m_BodyPivotPoint = new GameObject("BossPivotPoint").transform;
        m_BodyPivotPoint.position = m_PivotPos;

        m_PosY = transform.position.y;
    }

    private void Update()
    {
        transform.position = GetPosFromInput(transform.position, m_Input * m_MoveSpeed);
        FixHorizontalAngle();

        Vector3 lookAtPos = m_RoomPivotPoint.position;
        lookAtPos.y = m_PosY;
        transform.LookAt(lookAtPos);
    }

    public Vector3 GetPosFromInput(Vector3 current, Vector2 direction)
    {
        m_BodyPivotPoint.eulerAngles += Vector3.up * direction.x * Time.deltaTime;

        Vector3 targetPos = m_BodyPivotPoint.position + m_BodyPivotPoint.forward * m_Distance;

        m_PosY += direction.y * m_VerticalSpeedRatio * Time.deltaTime;
        targetPos.y = m_PosY;

        return targetPos;
    }

    void FixHorizontalAngle()
    {
        float y = m_BodyPivotPoint.eulerAngles.y;

        // Convertit en angle signé [-180, 180]
        float signedY = Mathf.DeltaAngle(0f, y);

        // Angle cible relatif à l’offset
        float target = Mathf.Clamp(
            signedY - m_AngleOffset,
            -m_MaxHorizontalAngle,
            m_MaxHorizontalAngle
        );

        // Reconstruit l’angle final
        float finalY = m_AngleOffset + target;

        // Applique en repassant en [0, 360)
        m_BodyPivotPoint.eulerAngles = new Vector3(
            m_BodyPivotPoint.eulerAngles.x,
            finalY,
            m_BodyPivotPoint.eulerAngles.z
        );
    }

    public Vector3 GetUpwardDir(Vector3 position)
    {
        Vector3 pos = (position - m_RoomPivotPoint.position).normalized;
        pos.y = position.y;
        return pos;
    }

    public Vector3 ApplyOnCylinder(Vector3 position)
    {
        Vector3 pos = position;
        pos.y = m_RoomPivotPoint.position.y;
        m_RoomPivotPoint.LookAt(pos);

        Vector3 newPos = m_RoomPivotPoint.position + m_RoomPivotPoint.forward * m_Distance;
        newPos.y = position.y;

        return newPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_PivotPos, .5f);
        Gizmos.DrawRay(m_PivotPos, Quaternion.Euler(0f, m_AngleOffset, 0f) * Vector3.forward * m_Distance);

        Gizmos.color = Color.cyan;
        Vector3 dirLeft = Quaternion.Euler(0f, -m_MaxHorizontalAngle + m_AngleOffset, 0f) * Vector3.forward;
        Vector3 dirRight = Quaternion.Euler(0f, m_MaxHorizontalAngle + m_AngleOffset, 0f) * Vector3.forward;

        Gizmos.DrawLine(m_PivotPos, m_PivotPos + dirLeft * m_Distance);
        Gizmos.DrawLine(m_PivotPos, m_PivotPos + dirRight * m_Distance);

        Gizmos.color = Color.blue;
        MVsGizmos.DrawCircle(m_PivotPos, m_Distance, Vector3.up);
    }
}