using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

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

    [Space]
    [SerializeField] Color m_ValidColor;
    [SerializeField] Color m_NotValidColor;

    bool m_InputPress;
    bool m_CanTouchTarget = false;

    [Header("References")]
    [SerializeField] MeshRenderer m_PreShowCircle;
    [SerializeField] MeshRenderer m_PreShowTriangle;
    [SerializeField] TMP_Text m_PercentageTxt;

    [SerializeField] RSO_PlayerAimTarget m_AimTarget;
    [SerializeField] RSO_PlayerController m_PlayerController;
    [SerializeField] RSO_PlayerCameraController m_Camera;

    [Space(10)]
    [SerializeField] InputActionReference m_AttackIA;

    [Header("Output")]
    [SerializeField] RSO_CameraTargetType m_TargetType;

    [Space(10)]
    [SerializeField] RSE_SetFreeLookCamTargetPos m_SetFreeLookCamTargetPos;
    [SerializeField] InputActionReference m_MousePosIA;
    [SerializeField] RSO_CurrentInputDeviceType m_CurrentInputDevice;

    public int Cost => m_ShootCost;

    private void OnEnable()
    {
        m_AttackIA.action.started += Attack;
    }
    private void OnDisable()
    {
        m_AttackIA.action.started -= Attack;
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
        if (m_InputPress) return;

        m_InputPress = true;
        m_PreShowCircle.gameObject.SetActive(true);
        m_TargetType.Set(CameraTargetType.FreeLook);

        if (m_CurrentInputDevice.Value == InputDeviceType.KeyboardMouse)
            m_SetFreeLookCamTargetPos.Call(m_MousePosIA.action.ReadValue<Vector2>() / new Vector2(Screen.width, Screen.height));
        else if (m_CurrentInputDevice.Value == InputDeviceType.Gamepad)
            m_SetFreeLookCamTargetPos.Call(
                m_Camera.Get().GetCamera().WorldToViewportPoint(
                    m_PlayerController.Get().transform.position));
    }

    public override void AttackEnd()
    {
        if (!m_InputPress) return;
        CancelAttack();
    }

    public void CancelAttack()
    {
        m_InputPress = false;
        m_PreShowCircle.gameObject.SetActive(false);
        m_TargetType.Set(CameraTargetType.AutoFocus);
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (m_InputPress && m_CanTouchTarget)
        {
            StartCoroutine(Attack());
            CancelAttack();
        }
    }

    public override IEnumerator Attack()
    {
        if (m_PlayerController.Get().GetPlayerMana().CurrentMana < m_ShootCost) yield break;
        m_PlayerController.Get().GetPlayerMana().Remove(m_ShootCost);

        Grenade grenade = PoolManager.Instance.Spawn(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        // Grenade grenade = Instantiate(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        grenade.Setup(m_AttackPoint.position, m_AimTarget.Get().position);

        grenade.Move();
    }

    void DrawPreShow()
    {
        if (!m_InputPress) return;

        m_PreShowCircle.transform.localScale = Vector3.one * (m_GrenadePrefab.GetRadius() * .2f);
        m_PercentageTxt.text = ((float)m_PlayerController.Get().GetPlayerMana().CurrentMana / m_PlayerController.Get().GetPlayerMana().MaxMana * 100).ToString("F0") + "%";

        if (m_PlayerController.Get().GetPlayerMana().CurrentMana < m_ShootCost)
        {
            m_PreShowCircle.material.color = m_NotValidColor;
            m_PreShowTriangle.material.color = m_NotValidColor;
            m_PercentageTxt.color = m_NotValidColor;
        }
        else
        {
            m_PreShowCircle.material.color = m_CanTouchTarget ? m_ValidColor : m_NotValidColor;
            m_PreShowTriangle.material.color = m_CanTouchTarget ? m_ValidColor : m_NotValidColor;
            m_PercentageTxt.color = m_CanTouchTarget ? m_ValidColor : m_NotValidColor;
        }
        
        m_PreShowCircle.transform.position = m_AimTarget.Get().position + Vector3.up * .1f;

        m_PreShowCircle.transform.localEulerAngles -= Vector3.up * (m_PreShowRotateSpeed * Time.deltaTime);
        m_PreShowTriangle.transform.localEulerAngles += Vector3.up * (m_PreShowRotateSpeed * 2 * Time.deltaTime);
        m_PercentageTxt.transform.eulerAngles = new Vector3(90, m_Camera.Get().GetCamera().transform.eulerAngles.y, 0);
    }
}