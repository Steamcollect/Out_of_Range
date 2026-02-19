using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BossMovementController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_MoveSpeed;

    [Space(10)]
    [SerializeField, Tooltip("Center of the rotation, the body will move around it")] Vector3 m_PivotPos;
    [SerializeField] float m_Distance;
    [SerializeField] float m_VerticalSpeedRatio = .2f;

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
        Gizmos.DrawRay(m_PivotPos, Vector3.forward * m_Distance);
    }
}