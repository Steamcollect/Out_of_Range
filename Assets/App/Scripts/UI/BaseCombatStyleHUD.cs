using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseCombatStyleHUD : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] protected Color m_ShootColor;
    [SerializeField] protected Color m_ReloadColor;
    [SerializeField] protected Color m_OverloadBuffColor;
    [SerializeField] protected Color m_OverloadNerfColor;
    [SerializeField] protected Color m_OverloadResetColor;

    [Header("Feedback Animation")]
    [SerializeField] protected float m_FeedbackAnimTime;
    [SerializeField] protected Image m_FeedbackImage;

    [Header("UI References")]
    [SerializeField] protected Image m_FillImg;
    [SerializeField] protected RectTransform m_CursorRct;
    [SerializeField] protected TMP_Text m_ReloadTxt;

    [Space(10)]
    [SerializeField] protected RectTransform m_ParentRect;
    [SerializeField] protected RectTransform m_BuffZoneRct;
    [SerializeField] protected RectTransform m_ResetZoneRct;

    protected float m_ParentWidth;
    protected CombatStyle m_CurrentStyle;
    protected OverloadCombatStyle m_OverloadStyle;

    protected virtual void BindToCombatStyle(CombatStyle style)
    {
        if (!style) return;

        Unbind();

        m_CurrentStyle = style;
        m_CurrentStyle.OnAmmoChange += SetFillValue;

        if (style is OverloadCombatStyle overloadStyle)
        {
            m_OverloadStyle = overloadStyle;
            m_OverloadStyle.OnOverloadStart += EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd += DisableReloadSkills;
            m_OverloadStyle.OnOverloadStateChange += OnOverloadStateChange;

            SetReloadSkillsRect();
        }
        else
        {
            m_OverloadStyle = null;
        }
    }

    protected virtual void Unbind()
    {
        if (m_CurrentStyle)
        {
            m_CurrentStyle.OnAmmoChange -= SetFillValue;
        }

        if (m_OverloadStyle)
        {
            m_OverloadStyle.OnOverloadStart -= EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd -= DisableReloadSkills;
            m_OverloadStyle.OnOverloadStateChange -= OnOverloadStateChange;
        }

        m_CurrentStyle = null;
        m_OverloadStyle = null;
    }

    protected virtual void SetFillValue(float value, float max)
    {
        float normalizedValue = Mathf.Clamp(value, 0, max) / max;
        m_FillImg.fillAmount = normalizedValue;
        
        if (m_CursorRct)
        {
            m_CursorRct.anchoredPosition = new Vector2(
                (normalizedValue * m_ParentWidth),
                m_CursorRct.anchoredPosition.y);
        }

        if (!m_OverloadStyle) return;

        UpdateVisualsByState(m_OverloadStyle.GetState(), normalizedValue);
    }

    protected virtual void UpdateVisualsByState(OverloadWeaponState state, float normalizedValue)
    {
        switch (state)
        {
            case OverloadWeaponState.CanShoot:
                m_FillImg.color = m_ShootColor;
                m_ReloadTxt.text = normalizedValue > .5f ? "R" : string.Empty;
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

    protected virtual void SetReloadSkillsRect()
    {
        if (!m_OverloadStyle) return;

        Vector2 buffValues = m_OverloadStyle.GetRangeToBuff();
        Vector2 resetValues = m_OverloadStyle.GetRangeToReset();
        m_ParentWidth = m_ParentRect.rect.width;

        if (m_BuffZoneRct)
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

        if (m_ResetZoneRct)
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

    protected virtual void EnableReloadSkills()
    {
        m_ResetZoneRct.gameObject.SetActive(true);
        m_BuffZoneRct.gameObject.SetActive(true);
    }

    protected virtual void DisableReloadSkills()
    {
        m_ResetZoneRct.gameObject.SetActive(false);
        m_BuffZoneRct.gameObject.SetActive(false);
    }

    protected virtual void OnOverloadStateChange(OverloadWeaponState state)
    {
        if (!m_FeedbackImage) return;

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

    protected virtual void OnDestroy()
    {
        Unbind();
    }
}

