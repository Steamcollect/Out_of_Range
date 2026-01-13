using System.Collections;
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
    [SerializeField] float m_PreShowRotateSpeed;

    bool m_InputPress;
    bool m_CanTouchTarget = false;

    [Header("References")]
    [SerializeField] MeshRenderer m_PreShowCircle;
    [SerializeField] MeshRenderer m_PreShowTriangle;

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
        m_PreShowCircle.gameObject.SetActive(true);
    }

    public override void AttackEnd(InputAction.CallbackContext ctx)
    {
        m_InputPress = false;
        if(m_CanTouchTarget) StartCoroutine(Attack());

        m_PreShowCircle.gameObject.SetActive(false);
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
        m_PreShowCircle.transform.localScale = Vector3.one * m_GrenadePrefab.GetRadius() * .2f;

        m_PreShowCircle.material.color = m_CanTouchTarget ? Color.green : Color.red;
        m_PreShowTriangle.material.color = m_CanTouchTarget ? Color.green : Color.red;

        m_PreShowCircle.transform.position = m_AimTarget.Get().position;

        m_PreShowCircle.transform.localEulerAngles -= Vector3.up * m_PreShowRotateSpeed * Time.deltaTime;
        m_PreShowTriangle.transform.parent.localEulerAngles += Vector3.up * m_PreShowRotateSpeed * 2 * Time.deltaTime;
    }

    public void AddAmmo(int count) => m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo + count, 0, m_MaxAmmo);
}