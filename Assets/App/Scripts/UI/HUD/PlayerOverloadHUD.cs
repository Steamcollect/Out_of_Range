using UnityEngine;
using UnityEngine.UI;

public class PlayerOverloadHUD : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_CircleRadius;

    [Header("Colors")]
    [SerializeField] Color m_ShootColor;
    [SerializeField] Color m_ReloadColor;
    [SerializeField] Color m_OverloadBuffColor;
    [SerializeField] Color m_OverloadNerfColor;
    [SerializeField] Color m_OverloadResetColor;

    [Header("UI References")]
    [SerializeField] Image m_BackgroundImg;
    [SerializeField] Image m_FillImg;
    [SerializeField] Image m_BlueImg;
    [SerializeField] Image m_YellowImg;

    [SerializeField] GameObject m_ReloadTextGO;

    [Space(5)]
    [SerializeField] RectTransform m_CursorRct;
    
    OverloadCombatStyle m_OverloadStyle;
    
    [Header("References")]
    [SerializeField] RSO_PlayerController m_Player;

    private void OnEnable()
    {
        m_Player.Get().GetPlayerCombat().OnPrimaryCombatStyleChange += InitBindings;
    }

    private void OnDisable()
    {
        Unbind();
        m_Player.Get().GetPlayerCombat().OnPrimaryCombatStyleChange -= InitBindings;
    }

    private void Start()
    {
        m_BlueImg.color = m_OverloadResetColor;
        m_YellowImg.color = m_OverloadBuffColor;
    }

    private void InitBindings()
    {
        BindToCombatStyle(m_Player.Get().GetPlayerCombat().GetPrimaryCombatStyle());
    }

    protected virtual void BindToCombatStyle(CombatStyle style)
    {
        if (!style) return;

        Unbind();

        if (style is OverloadCombatStyle overloadStyle)
        {
            m_OverloadStyle = overloadStyle;

            m_OverloadStyle.OnAmmoChange += SetFillValue;
            m_OverloadStyle.OnOverloadStart += EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd += DisableReloadSkills;
            m_OverloadStyle.OnOverloadStateChange += OnOverloadStateChange;

            SetupFills();
        }
        else
        {
            m_OverloadStyle = null;
        }
    }

    protected virtual void Unbind()
    {
        if (m_OverloadStyle)
        {
            m_OverloadStyle.OnAmmoChange -= SetFillValue;
        }

        if (m_OverloadStyle)
        {
            m_OverloadStyle.OnOverloadStart -= EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd -= DisableReloadSkills;
            m_OverloadStyle.OnOverloadStateChange -= OnOverloadStateChange;
        }

        m_OverloadStyle = null;
        m_OverloadStyle = null;
    }

    protected virtual void SetFillValue(float value, float max)
    {
        float normalizedValue = Mathf.Clamp(value, 0, max) / max;
        float valueOnSlider = normalizedValue * m_BackgroundImg.fillAmount;

        m_FillImg.fillAmount = normalizedValue * m_BackgroundImg.fillAmount;

        float tot = 360 * m_BackgroundImg.fillAmount;
        float angle = m_BackgroundImg.rectTransform.eulerAngles.z;

        m_CursorRct.anchoredPosition = Quaternion.Euler(0, 0, angle - tot * normalizedValue) * Vector3.up * m_CircleRadius;
        m_CursorRct.rotation = Quaternion.Euler(0, 0, angle - tot * normalizedValue);

        if (!m_OverloadStyle) return;

        UpdateVisualsByState(m_OverloadStyle.GetState(), normalizedValue);
    }

    protected virtual void UpdateVisualsByState(OverloadWeaponState state, float normalizedValue)
    {
        m_ReloadTextGO.SetActive(false);
        
        switch (state)
        {
            case OverloadWeaponState.CanShoot:
                m_FillImg.color = m_ShootColor;
                m_ReloadTextGO?.SetActive(normalizedValue > .5f);
                break;

            case OverloadWeaponState.DefaultCool:
                m_FillImg.color = m_ReloadColor;
                break;

            case OverloadWeaponState.CoolBuffed:
                m_FillImg.color = m_OverloadBuffColor;
                break;

            case OverloadWeaponState.CoolNerfed:
                m_FillImg.color = m_OverloadNerfColor;
                break;

            case OverloadWeaponState.OverloadCool:
                m_FillImg.color = m_ReloadColor;
                break;
        }
    }

    protected virtual void SetupFills()
    {
        if (!m_OverloadStyle) return;

        Vector2 buffValues = m_OverloadStyle.GetRangeToBuff();
        Vector2 resetValues = m_OverloadStyle.GetRangeToReset();

        float tot = 360 * m_BackgroundImg.fillAmount;
        float angle = m_BackgroundImg.rectTransform.eulerAngles.z;

        m_FillImg.rectTransform.eulerAngles = new Vector3(0, 0, angle);
        m_FillImg.fillAmount = 0;

        m_BlueImg.rectTransform.eulerAngles = new Vector3(0, 0, angle - tot * (resetValues.x * .01f));
        m_BlueImg.fillAmount = (resetValues.y - resetValues.x) * .01f * m_BackgroundImg.fillAmount;

        m_YellowImg.rectTransform.eulerAngles = new Vector3(0, 0, angle - tot * (buffValues.x * .01f));
        m_YellowImg.fillAmount = (buffValues.y - buffValues.x) * .01f * m_BackgroundImg.fillAmount;
    }

    protected virtual void EnableReloadSkills()
    {
        m_BlueImg.gameObject.SetActive(true);
        m_YellowImg.gameObject.SetActive(true);
    }
    protected virtual void DisableReloadSkills()
    {
        m_BlueImg.gameObject.SetActive(false);
        m_YellowImg.gameObject.SetActive(false);
    }

    protected virtual void OnOverloadStateChange(OverloadWeaponState state)
    {
        
    }

    protected virtual void OnDestroy()
    {
        Unbind();
    }
}

