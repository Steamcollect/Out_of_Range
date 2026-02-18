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
    Transform m_PivotPoint;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_PivotPoint = new GameObject("BossPivotPoint").transform;
        m_PivotPoint.position = m_PivotPos;

        m_PosY = transform.position.y;
    }

    private void Update()
    {
        transform.position = GetPosFromInput(transform.position, m_Input * m_MoveSpeed);

        Vector3 lookAtPos = m_PivotPoint.position;
        lookAtPos.y = m_PosY;
        transform.LookAt(lookAtPos);
    }

    public Vector3 GetPosFromInput(Vector3 current, Vector2 direction)
    {
        m_PivotPoint.eulerAngles += Vector3.up * direction.x * Time.deltaTime;
        Vector3 targetPos = m_PivotPoint.position + m_PivotPoint.forward * m_Distance;

        m_PosY += direction.y * m_VerticalSpeedRatio * Time.deltaTime;
        targetPos.y = m_PosY;

        return targetPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_PivotPos, .5f);
        Gizmos.DrawRay(m_PivotPos, Vector3.forward * m_Distance);
    }
}