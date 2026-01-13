using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatStyleHUD : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 m_PosOffset;

    [Space(10)]
    [SerializeField] Color m_ShootColor;
    [SerializeField] Color m_ReloadColor;
    [SerializeField] Color m_OverloadBuffColor;
    [SerializeField] Color m_OverloadNerfColor;
    [SerializeField] Color m_OverloadResetColor;

    float m_ParentWidth;

    [Space(10)]
    [SerializeField] float m_FeedbackAnimTime;
    [SerializeField] Image m_FeedbackImage;

    [Header("References")]
    [SerializeField] Image m_FillImg;
    [SerializeField] RectTransform m_CursorRct;

    [SerializeField] TMP_Text m_ReloadTxt;
    [SerializeField] RSO_PlayerCameraController m_PlayerCameraController;

    CombatStyle m_CurrentStyle;
    OverloadCombatStyle m_OverloadStyle;

    [Space(10)]
    [SerializeField] RectTransform m_ParentRect;
    [SerializeField] RectTransform m_BuffZoneRct;
    [SerializeField] RectTransform m_ResetZoneRct;

    [Header("Input")]
    [SerializeField] RSO_PlayerController m_PlayerController;

    //[Header("Output")]

    private void OnEnable()
    {
        PlayerCombat c = m_PlayerController.Get().GetPlayerCombat();
        c.OnPrimaryCombatStyleChange += OnCombatStyleChange;
    }

    private void OnDisable()
    {
        PlayerCombat c = m_PlayerController.Get().GetPlayerCombat();
        c.OnPrimaryCombatStyleChange -= OnCombatStyleChange;
        
        if (combat != null)
        {
            combat.GetPrimaryCombatStyle().OnAmmoChange -= SetFillValue;

            m_CurrentStyle.OnAmmoChange -= SetFillValue;

            m_OverloadStyle.OnOverloadStart -= EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd -= DisableReloadSkills;
            m_OverloadStyle.OnOverloadStateChange -= OnStateChange;
        }
    }

    private PlayerCombat combat;
    void Init()
    {
        if(combat == null) combat = m_PlayerController.Get().GetPlayerCombat();
        m_CurrentStyle = combat.GetPrimaryCombatStyle();
        m_CurrentStyle.OnAmmoChange += SetFillValue;

        if (combat.GetPrimaryCombatStyle() is not OverloadCombatStyle) return;

        m_OverloadStyle = combat.GetPrimaryCombatStyle() as OverloadCombatStyle;
        m_OverloadStyle.OnOverloadStart += EnableReloadSkills;
        m_OverloadStyle.OnOverloadEnd += DisableReloadSkills;
        m_OverloadStyle.OnOverloadStateChange += OnStateChange;

        SetReloadSkillsRect();
    }
    private void LateUpdate()
    {
        UpdatePosition();
    }

    void OnCombatStyleChange()
    {
        if (combat != null)
        {
            combat.GetPrimaryCombatStyle().OnAmmoChange -= SetFillValue;

            m_CurrentStyle.OnAmmoChange -= SetFillValue;

            m_OverloadStyle.OnOverloadStart -= EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd -= DisableReloadSkills;
            m_OverloadStyle.OnOverloadStateChange -= OnStateChange;
        }

        Init();
    }


    public void SetFillValue(float value, float max)
    {
        float _value = Mathf.Clamp(value, 0, max) / max;
        m_FillImg.fillAmount = _value;
        m_CursorRct.anchoredPosition = new Vector2(
            (_value * m_ParentWidth),
            m_CursorRct.anchoredPosition.y);

        if (m_OverloadStyle == null) return;

        switch (m_OverloadStyle.GetState())
        {
            case OverloadWeaponState.CanShoot:
                m_FillImg.color = m_ShootColor;
                m_ReloadTxt.text = _value > .5f ? "R" : string.Empty;
                break;

            case OverloadWeaponState.DefaultCool:
                m_FillImg.color = m_ReloadColor;
                m_ReloadTxt.text = string.Empty;
                break;

            case OverloadWeaponState.CoolBuffed:
                m_FillImg.color = m_OverloadBuffColor;
                m_ReloadTxt.text = string.Empty;
                break;

            case OverloadWeaponState.CoolNerfed:
                m_FillImg.color = m_OverloadNerfColor;
                m_ReloadTxt.text = string.Empty;
                break;

            case OverloadWeaponState.OverloadCool:
                m_FillImg.color = m_ReloadColor;
                m_ReloadTxt.text = "LClick";
                break;
        }
    }

    void SetReloadSkillsRect()
    {
        Vector2 buffValues = m_OverloadStyle.GetRangeToBuff();
        Vector2 resetValues = m_OverloadStyle.GetRangeToReset();
        m_ParentWidth = m_ParentRect.rect.width;

        if (m_BuffZoneRct != null)
        {
            float left = buffValues.x * (m_ParentWidth * .01f);
            float right = buffValues.y * (m_ParentWidth * .01f);

            Vector2 offMin = m_BuffZoneRct.offsetMin;
            offMin.x = left;
            m_BuffZoneRct.offsetMin = offMin;

            Vector2 offMax = m_BuffZoneRct.offsetMax;
            offMax.x = -(m_ParentWidth - right);
            m_BuffZoneRct.offsetMax = offMax;
        }

        if (m_ResetZoneRct != null)
        {
            float left = resetValues.x * (m_ParentWidth * .01f);
            float right = resetValues.y * (m_ParentWidth * .01f);

            Vector2 offMin = m_ResetZoneRct.offsetMin;
            offMin.x = left;
            m_ResetZoneRct.offsetMin = offMin;

            Vector2 offMax = m_ResetZoneRct.offsetMax;
            offMax.x = -(m_ParentWidth - right);
            m_ResetZoneRct.offsetMax = offMax;
        }
    }

    void EnableReloadSkills()
    {
        m_ResetZoneRct.gameObject.SetActive(true);
        m_BuffZoneRct.gameObject.SetActive(true);
    }
    void DisableReloadSkills()
    {
        m_ResetZoneRct.gameObject.SetActive(false);
        m_BuffZoneRct.gameObject.SetActive(false);
    }

    void UpdatePosition()
    {
        transform.position = (Vector2)m_PlayerCameraController.Get().GetCamera()
            .WorldToScreenPoint(m_PlayerController.Get().GetTargetPosition()) + m_PosOffset;
    }

    void OnStateChange(OverloadWeaponState state)
    {
        m_FeedbackImage.transform.localScale = Vector3.one;
        switch (state)
        {
            case OverloadWeaponState.CanShoot:
                m_FeedbackImage.color = m_OverloadResetColor;
                break;

            case OverloadWeaponState.CoolBuffed:
                m_FeedbackImage.color = m_OverloadBuffColor;
                break;

            case OverloadWeaponState.CoolNerfed:
                m_FeedbackImage.color = m_OverloadNerfColor;
                break;
        }

        m_FeedbackImage.gameObject.SetActive(true);

        m_FeedbackImage.transform.DOScale(new Vector3(1.15f, 3.3f, 1), m_FeedbackAnimTime);
        m_FeedbackImage.DOFade(0, m_FeedbackAnimTime).OnComplete(() =>
        {
            m_FeedbackImage.gameObject.SetActive(false);
        });
    }
}