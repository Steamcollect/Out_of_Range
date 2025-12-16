using System;
using Sirenix.OdinInspector;
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

    [Header("References")]
    [SerializeField] Image m_FillImg;
    [SerializeField] RectTransform m_CursorRct;

    [SerializeField] TMP_Text m_ReloadTxt;
    [SerializeField] RSO_PlayerCameraController m_PlayerCameraController;

    RangeOverload_CombatStyle m_OverloadStyle;

    [Space(10)]
    [SerializeField] RectTransform m_ParentRect;
    [SerializeField] RectTransform m_BuffZoneRct;
    [SerializeField] RectTransform m_ResetZoneRct;

    [Header("Input")]
    [SerializeField] RSO_PlayerController m_PlayerController;

    //[Header("Output")]

    private void OnEnable()
    {
        PlayerCombat c = m_PlayerController.Get().GetCombat() as PlayerCombat;
        c.OnPrimaryCombatStyleChange += OnCombatStyleChange;
    }

    private void OnDisable()
    {
        PlayerCombat c = m_PlayerController.Get().GetCombat() as PlayerCombat;
        c.OnPrimaryCombatStyleChange -= OnCombatStyleChange;
        
        if (combat != null)
        {
            combat.GetPrimaryCombatStyle().OnAmmoChange -= SetFillValue;
        }
        
        if (m_OverloadStyle != null)
        {
            m_OverloadStyle.OnOverloadStart -= EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd -= DisableReloadSkills;
        }
    }

    private PlayerCombat combat;
    void Init()
    {
        combat = m_PlayerController.Get().GetCombat() as PlayerCombat;

        combat.GetPrimaryCombatStyle().OnAmmoChange += SetFillValue;

        m_OverloadStyle = combat.GetPrimaryCombatStyle() as RangeOverload_CombatStyle;
        if (m_OverloadStyle == null) return;

        m_OverloadStyle.OnOverloadStart += EnableReloadSkills;
        m_OverloadStyle.OnOverloadEnd += DisableReloadSkills;

        Vector2 buffValues = m_OverloadStyle.RangeToBuff;
        Vector2 resetValues = m_OverloadStyle.RangeToReset;
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

    void OnCombatStyleChange()
    {
        if (combat != null)
        {
            combat.GetPrimaryCombatStyle().OnAmmoChange -= SetFillValue;
        }
        
        if (m_OverloadStyle != null)
        {
            m_OverloadStyle.OnOverloadStart -= EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd -= DisableReloadSkills;
        }
        
        Init();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    public void SetFillValue(float value, float max)
    {
        Debug.Log("SetFillValue: " + value + " / " + max);
        float _value = Mathf.Clamp(value, 0, max) / max;
        m_FillImg.fillAmount = _value;
        m_CursorRct.anchoredPosition = new Vector2(
            (_value * m_ParentWidth),
            m_CursorRct.anchoredPosition.y);

        if (m_OverloadStyle == null) return;

        switch (m_OverloadStyle.GetState())
        {
            case RangeOverloadWeaponState.CanShoot:
                m_FillImg.color = m_ShootColor;
                break;

                case RangeOverloadWeaponState.DefaultCool:
                m_FillImg.color = m_ReloadColor;
                break;

                case RangeOverloadWeaponState.CoolBuffed:
                m_FillImg.color = m_OverloadBuffColor;
                break;

                case RangeOverloadWeaponState.CoolNerfed:
                m_FillImg.color = m_OverloadNerfColor;
                break;

                case RangeOverloadWeaponState.OverloadCool:
                m_FillImg.color = m_ReloadColor;
                break;
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
}