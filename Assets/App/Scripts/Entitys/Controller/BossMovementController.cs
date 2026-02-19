using System.Collections;
using MVsToolkit.Utilities;
using UnityEngine;

public class BossMovementController : MonoBehaviour, IMovement
{
    [Header("Settings")]
    [SerializeField] float m_MoveSpeed;
    [SerializeField] float m_SpeedMult = 1;

    [Space(10)]
    [SerializeField] Vector2 m_MovementSpacing;
    [SerializeField] Vector2 m_MovementTime;

    [Space(10)]
    [SerializeField, Tooltip("Center of the rotation, the body will move around it")] Vector3 m_PivotPos;
    [SerializeField] float m_Distance;
    [SerializeField] float m_VerticalSpeedRatio = .2f;

    [Space(10)]
    [SerializeField, Range(0, 360)] int m_MaxHorizontalAngle;
    [SerializeField] int m_AngleOffset;

    float m_PosY;
    bool m_IsLockLeft, m_IsLockRight;

    [Header("References")]
    [SerializeField] Transform m_Body;
    [SerializeField] Rigidbody m_Rb;
    
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

        m_PosY = m_Body.position.y;
    }

    void Start()
    {
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        yield return new WaitForSeconds(Random.Range(m_MovementSpacing.x, m_MovementSpacing.y));

        float x = 0;
        if (m_IsLockLeft) x = 1;
        else if (m_IsLockRight) x = -1;
        else x = Random.value > .5f ? 1 : -1;

        float t = 0;

        float m_Time = Random.Range(m_MovementTime.x, m_MovementTime.y);
        while (t < m_Time)
        {
            t += Time.deltaTime;
            yield return null;

            Vector2 dir = new Vector2(x, 0) * m_MoveSpeed * m_SpeedMult;

            m_BodyPivotPoint.eulerAngles += Vector3.up * dir.x * Time.deltaTime;

            Vector3 targetPos = m_BodyPivotPoint.position + m_BodyPivotPoint.forward * m_Distance;

            m_PosY += dir.y * m_VerticalSpeedRatio * Time.deltaTime;
            targetPos.y = m_PosY;

            m_Body.position = targetPos;
            FixHorizontalAngle(ref m_IsLockLeft, ref m_IsLockRight);

            if (m_IsLockLeft || m_IsLockRight)
            {
                StartCoroutine(Movement());
                yield break;
            }

            Vector3 lookAtPos = m_RoomPivotPoint.position;
            lookAtPos.y = m_PosY;
            m_Body.LookAt(lookAtPos);
        }

        StartCoroutine(Movement());
    }

    void FixHorizontalAngle(ref bool lockLeft, ref bool lockRight)
    {
        float y = m_BodyPivotPoint.eulerAngles.y;

        // Convertit en angle signé [-180, 180]
        float signedY = Mathf.DeltaAngle(0f, y);

        // Angle relatif à l’offset
        float relative = signedY - m_AngleOffset;

        // Détection des locks
        if (relative <= -m_MaxHorizontalAngle)
        {
            lockLeft = true;
            lockRight = false;
        }
        else if (relative >= m_MaxHorizontalAngle)
        {
            lockRight = true;
            lockLeft = false;
        }
        else
        {
            lockLeft = false;
            lockRight = false;
        }

        // Clamp final
        float clamped = Mathf.Clamp(relative, -m_MaxHorizontalAngle, m_MaxHorizontalAngle);
        float finalY = m_AngleOffset + clamped;

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

    public void Move(Vector3 input) { }
    public void ResetVelocity() { }
    public void SetSpeedMult(float mult)
    {
        m_SpeedMult = mult;
    }
}