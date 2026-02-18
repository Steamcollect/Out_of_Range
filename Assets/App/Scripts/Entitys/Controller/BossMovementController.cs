using UnityEngine;

public class BossMovementController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_MoveSpeed;

    [SerializeField, Tooltip("Center of the rotation, the body will move around it")] Vector3 m_PivotPos;
    [SerializeField] float m_Distance;

    public Vector2 m_Input;

    //[Header("References")]
    Transform m_PivotPoint;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_PivotPoint = new GameObject("BossPivotPoint").transform;
        m_PivotPoint.position = m_PivotPos;
    }

    private void Update()
    {
        m_PivotPoint.eulerAngles += new Vector3(m_Input.y, m_Input.x, 0) * m_MoveSpeed * Time.deltaTime;
        transform.position = m_PivotPoint.position + m_PivotPoint.forward * m_Distance;
        transform.LookAt(m_PivotPoint.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_PivotPos, .5f);
        Gizmos.DrawRay(m_PivotPos, Vector3.forward * m_Distance);
    }
}