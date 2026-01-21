using System.Collections;
using UnityEngine;

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
    [SerializeField] RSO_Mana m_Mana;

    [SerializeField] MeshRenderer m_PreShowCircle;
    [SerializeField] MeshRenderer m_PreShowTriangle;

    [SerializeField] RSO_PlayerAimTarget m_AimTarget;
    
    [Header("Output")]
    [SerializeField] RSO_CameraTargetType m_TargetType;

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
        m_InputPress = false;
        if(m_CanTouchTarget) StartCoroutine(Attack());

        m_PreShowCircle.gameObject.SetActive(false);
        m_TargetType.Set(CameraTargetType.AutoFocus);
    }

    public override IEnumerator Attack()
    {
        if (!m_Mana.Get().HaveEngough(m_ShootCost)) yield break;
        m_Mana.Get().Remove(m_ShootCost);
        
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

        m_PreShowCircle.transform.position = m_AimTarget.Get().position + Vector3.up * .1f;

        m_PreShowCircle.transform.localEulerAngles -= Vector3.up * m_PreShowRotateSpeed * Time.deltaTime;
        m_PreShowTriangle.transform.parent.localEulerAngles += Vector3.up * m_PreShowRotateSpeed * 2 * Time.deltaTime;
    }
}