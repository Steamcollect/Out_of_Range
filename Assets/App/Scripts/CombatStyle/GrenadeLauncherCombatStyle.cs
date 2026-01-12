using System.Collections;
using MoreMountains.Tools;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeLauncherCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] Grenade m_GrenadePrefab;
    [SerializeField] Transform m_AttackPoint;

    [Space(5)]
    [SerializeField] int m_MaxAmmo;
    [SerializeField, ReadOnly] int m_CurrentAmmo;

    [Space(5)]
    [SerializeField] LayerMask m_UnpassingWallMask;

    [Space(10)]
    [SerializeField] int m_PreShowLinePointsCount = 10;

    bool m_InputPress;
    bool m_CanTouchTarget = false;

    [Header("References")]
    [SerializeField] MeshRenderer m_PreShowCylinder;
    [SerializeField] LineRenderer m_PreShowLine;

    [SerializeField] RSO_PlayerAimTarget m_AimTarget;

    private void Start()
    {
        m_CurrentAmmo = m_MaxAmmo;
    }

    private void FixedUpdate()
    {
        if (m_InputPress)
        {
            Vector3 s = m_AttackPoint.position;
            Vector3 e = m_AimTarget.Get().position;
            m_CanTouchTarget = !Physics.Raycast(s, e, out RaycastHit hit, Vector3.Distance(e, s), m_UnpassingWallMask);

            DrawPreShow();
        }
    }

    public override void AttackStart(InputAction.CallbackContext ctx)
    {
        m_InputPress = true;

        m_PreShowCylinder.gameObject.SetActive(true);
        m_PreShowLine.gameObject.SetActive(true);
    }

    public override void AttackEnd(InputAction.CallbackContext ctx)
    {
        m_InputPress = false;
        if(m_CanTouchTarget) StartCoroutine(Attack());

        m_PreShowCylinder.gameObject.SetActive(false);
        m_PreShowLine.gameObject.SetActive(false);
    }

    public override IEnumerator Attack()
    {
        if (m_CurrentAmmo <= 0) yield break;
        m_CurrentAmmo--;
        
        Grenade grenade = Instantiate(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        grenade.Setup(m_AttackPoint.position, m_AimTarget.Get().position);

        grenade.Move();

        yield break;
    }

    void DrawPreShow()
    {
        m_PreShowCylinder.transform.localScale = new Vector3(m_GrenadePrefab.GetRadius(), .05f, m_GrenadePrefab.GetRadius());

        m_PreShowCylinder.material.color = m_CanTouchTarget ? Color.green : Color.red;
        m_PreShowLine.material.color = m_CanTouchTarget ? Color.green : Color.red;

        m_PreShowLine.positionCount = m_PreShowLinePointsCount;

        Vector3 s = m_AttackPoint.position;
        Vector3 e = m_AimTarget.Get().position;

        m_PreShowLine.SetPosition(0, s);

        for (int i = 0; i < m_PreShowLinePointsCount - 2; i++)
        {
            float t = (float)i / (float)m_PreShowLinePointsCount;
            m_PreShowLine.SetPosition(i + 1, new Vector3(
                Mathf.Lerp(s.x, e.x, t),
                Mathf.Lerp(s.y, e.y, t) + m_GrenadePrefab.GetMovementCurve().Evaluate(t) * m_GrenadePrefab.GetMinHeight(),
                Mathf.Lerp(s.z, e.z, t)));
        }

        m_PreShowLine.SetPosition(m_PreShowLinePointsCount - 1, e);
        m_PreShowCylinder.transform.position = e;
    }

    public void AddAmmo(int count) => m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo + count, 0, m_MaxAmmo);
}