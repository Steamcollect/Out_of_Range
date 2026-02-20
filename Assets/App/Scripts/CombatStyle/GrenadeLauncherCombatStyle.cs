using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeLauncherCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] int m_ShootCost;

    [Space(10)]
    [SerializeField] Grenade m_GrenadePrefab;
    [SerializeField] Transform m_AttackPoint;

    [Space(5)]
    [SerializeField] LayerMask m_UnpassingWallMask;

    [Space(10)]
    [SerializeField] float m_PreShowRotateSpeed;

    bool m_InputPress;
    bool m_CanTouchTarget = false;

    [Header("References")]
    [SerializeField] MeshRenderer m_PreShowCircle;
    [SerializeField] MeshRenderer m_PreShowTriangle;

    [SerializeField] RSO_PlayerAimTarget m_AimTarget;
    [SerializeField] RSO_PlayerController m_PlayerController;

    [Space(10)]
    [SerializeField] InputActionReference m_CancelAttackIA;

    [Header("Output")]
    [SerializeField] RSO_CameraTargetType m_TargetType;
    
    public int Cost => m_ShootCost;

    private void OnEnable()
    {
        m_CancelAttackIA.action.started += CancelAttack;
    }
    private void OnDisable()
    {
        m_CancelAttackIA.action.started -= CancelAttack;
    }

    private void FixedUpdate()
    {
        if (m_InputPress)
        {
            Vector3 s = m_AttackPoint.position;
            Vector3 e = m_AimTarget.Get().position;

            Debug.DrawLine(s, e, Color.blue);
            m_CanTouchTarget = !Physics.Linecast(s, e, m_UnpassingWallMask);
        }
    }
    
    private void LateUpdate()
    {
        DrawPreShow();
    }

    public override void AttackStart()
    {
        m_InputPress = true;
        m_PreShowCircle.gameObject.SetActive(true);
        m_TargetType.Set(CameraTargetType.FreeLook);
    }

    public override void AttackEnd()
    {
        if (!m_InputPress) return;

        m_InputPress = false;
        if(m_CanTouchTarget) StartCoroutine(Attack());

        m_PreShowCircle.gameObject.SetActive(false);
        m_TargetType.Set(CameraTargetType.AutoFocus);
    }

    public void CancelAttack(InputAction.CallbackContext ctx)
    {
        m_InputPress = false;
        m_PreShowCircle.gameObject.SetActive(false);
        m_TargetType.Set(CameraTargetType.AutoFocus);
    }

    public override IEnumerator Attack()
    {
        if (m_PlayerController.Get().GetPlayerMana().CurrentMana < m_ShootCost) yield break;
        m_PlayerController.Get().GetPlayerMana().Remove(m_ShootCost);
        
        Grenade grenade = Instantiate(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        grenade.Setup(m_AttackPoint.position, m_AimTarget.Get().position);

        grenade.Move();
    }

    void DrawPreShow()
    {
        m_PreShowCircle.transform.localScale = Vector3.one * (m_GrenadePrefab.GetRadius() * .2f);

        if (m_PlayerController.Get().GetPlayerMana().CurrentMana < m_ShootCost)
        {
            m_PreShowCircle.material.color = Color.gray;
            m_PreShowTriangle.material.color = Color.gray;
        }
        else
        {
            m_PreShowCircle.material.color = m_CanTouchTarget ? Color.green : Color.red;
            m_PreShowTriangle.material.color = m_CanTouchTarget ? Color.green : Color.red;
        }
        
        

        m_PreShowCircle.transform.position = m_AimTarget.Get().position + Vector3.up * .1f;

        m_PreShowCircle.transform.localEulerAngles -= Vector3.up * (m_PreShowRotateSpeed * Time.deltaTime);
        m_PreShowTriangle.transform.parent.localEulerAngles += Vector3.up * (m_PreShowRotateSpeed * 2 * Time.deltaTime);
    }
}