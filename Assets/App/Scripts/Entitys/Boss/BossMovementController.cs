using System.Collections;
using UnityEngine;

public class BossMovementController : MonoBehaviour, IMovement
{
    [Header("Settings")]
    [SerializeField] float m_MoveSpeed;
    [SerializeField] float m_SpeedMult = 1;
    [SerializeField, Range(0, 1)] float m_VerticalSpeedRatio = .2f;
    [SerializeField] float m_TimeAterSpawn = 1;

    [Space(10)]
    [SerializeField] Vector2 m_MovementSpacing;
    [SerializeField] Vector2 m_MovementTime;

    [Space(10)]
    [SerializeField] bool m_CanMove = false;
    bool m_IsOvertakingLeft = false, m_IsOvertakingRight = false;

    [Header("References")]
    [SerializeField] Transform m_Body;
    [SerializeField] BossEnemyController m_Controller;

    [Header("Input")]
    [SerializeField] RSE_SetBossCanMove m_SetCanMove;
    
    //[Header("Output")]

    void OnEnable()
    {
        m_SetCanMove.Action += SetCanMove;
    }
    void OnDisable()
    {
        m_SetCanMove.Action -= SetCanMove;
    }

    void Start()
    {
        StartCoroutine(Spawn());
    }

    void Move(Vector2 input)
    {
        if (input.x != 0f)
        {
            float angle = input.x * m_MoveSpeed * m_SpeedMult * Time.deltaTime;
            m_Body.RotateAround(
                BossMovementPivot.Instance.Pivot,
                Vector3.up,
                angle
            );
        }

        if (input.y != 0f)
        {
            Vector3 pos = m_Body.position;
            pos.y += input.y * m_MoveSpeed * m_SpeedMult * m_VerticalSpeedRatio * Time.deltaTime;
            m_Body.position = pos;
        }

        m_Body.position = BossMovementPivot.Instance.ApplyOnCylinder(m_Body.position);

        Vector3 look = BossMovementPivot.Instance.Pivot;
        look.y = m_Body.position.y;
        m_Body.LookAt(look);
    }

    IEnumerator Spawn()
    {
        m_Controller.CanHandlePaterns = false;

        Vector3 target = BossMovementPivot.Instance.CenterPos;

        while (true)
        {
            yield return null;

            Vector3 pos = m_Body.position;

            float verticalDir = Mathf.Sign(target.y - pos.y);

            Vector3 toBoss = (pos - BossMovementPivot.Instance.Pivot).normalized;
            Vector3 targetDir = BossMovementPivot.Instance.transform.forward;

            float signedAngle = Vector3.SignedAngle(toBoss, targetDir, Vector3.up);

            float horizontalDir = Mathf.Sign(signedAngle);

            Vector2 artificialInput = new Vector2(horizontalDir, verticalDir);

            Move(artificialInput);

            bool heightOK = Mathf.Abs(target.y - m_Body.position.y) < 0.01f;
            bool angleOK = Mathf.Abs(signedAngle) < 0.5f;

            if (heightOK && angleOK)
                break;
        }

        yield return new WaitForSeconds(m_TimeAterSpawn);

        m_Controller.CanHandlePaterns = true;
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        yield return new WaitUntil(() => m_CanMove);
        yield return new WaitForSeconds(Random.Range(m_MovementSpacing.x, m_MovementSpacing.y));

        float xDir = m_IsOvertakingLeft ? 1 : m_IsOvertakingRight ? -1 : Mathf.Sign(Random.Range(-1f, 1f));

        float duration = Random.Range(m_MovementTime.x, m_MovementTime.y);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;

            Vector2 input = new Vector2(xDir, 0f);
            Move(input);

            BossMovementPivot.Instance.IsPosOvertakingMaxAngles(
                m_Body.position,
                ref m_IsOvertakingLeft,
                ref m_IsOvertakingRight
            );

            if (m_IsOvertakingLeft || m_IsOvertakingRight)
            {
                StartCoroutine(Movement());
                yield break;
            }
        }

        StartCoroutine(Movement());
    }

    void SetCanMove(bool value) => m_CanMove = value;

    public void Move(Vector3 input) { }
    public void ResetVelocity() { }
    public void SetSpeedMult(float mult) => m_SpeedMult = mult;

    void IMovement.SetCanMove(bool canMove) { }
}