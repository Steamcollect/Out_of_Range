using DG.Tweening;
using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverloadHUD : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_CircleRadius;

    [Space]
    [SerializeField, Range(0, 1)] float m_WithoutManaFA = .41f;
    [SerializeField, Range(0, 1)] float m_WithManaFA = .345f;

    [Space]
    [SerializeField] float m_TimeBeforeHiding = 5;
    float m_HidingTimer = 0;
    bool m_IsHiding = true;

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
    [SerializeField] Image m_CursorImg;
    
    OverloadCombatStyle m_OverloadStyle;
    
    [Header("References")]
    [SerializeField] RSO_PlayerController m_Player;

    private void OnEnable()
    {
        m_Player.Get().GetPlayerCombat().OnPrimaryCombatStyleChange += InitBindings;
        m_Player.Get().GetPlayerCombat().OnSecondaryCombatStyleChange += UpdateUI;

        if(m_Player.Get().GetPlayerCombat().GetPrimaryCombatStyle() != null)
            InitBindings();
    }

    private void OnDisable()
    {
        Unbind();
        m_Player.Get().GetPlayerCombat().OnPrimaryCombatStyleChange -= InitBindings;
        m_Player.Get().GetPlayerCombat().OnSecondaryCombatStyleChange -= UpdateUI;
    }

    private void Start()
    {
        m_BlueImg.color = m_OverloadResetColor;
        m_YellowImg.color = m_OverloadBuffColor;
    }

    private void Update()
    {
        if (!m_IsHiding)
        {
            m_HidingTimer += Time.deltaTime;
            
            if(m_HidingTimer >= m_TimeBeforeHiding)
            {
                m_IsHiding = true;

                m_BackgroundImg.DOKill();
                m_CursorImg.DOKill();

                m_BackgroundImg.DOFade(0, 1);
                m_CursorImg.DOFade(0, 1);
            }
        }
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

            UpdateUI();
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
            m_OverloadStyle.OnOverloadStart -= EnableReloadSkills;
            m_OverloadStyle.OnOverloadEnd -= DisableReloadSkills;
        }

        m_OverloadStyle = null;
    }

    protected virtual void SetFillValue(float value, float max)
    {
        if (value > 0)
        {
            m_HidingTimer = 0;

            if (m_IsHiding)
            {
                m_IsHiding = false;

                m_BackgroundImg.DOKill();
                m_CursorImg.DOKill();

                m_BackgroundImg.DOFade(.8f, .1f);
                m_CursorImg.DOFade(1, .1f);
            }
        }

        float normalizedValue = Mathf.Clamp(value, 0, max) / max;
        float valueOnSlider = normalizedValue * m_BackgroundImg.fillAmount;

        m_FillImg.fillAmount = normalizedValue * m_BackgroundImg.fillAmount;

        float tot = 360 * m_BackgroundImg.fillAmount;
        float angle = m_BackgroundImg.rectTransform.eulerAngles.z;

        m_CursorImg.rectTransform.anchoredPosition = Quaternion.Euler(0, 0, angle - tot * normalizedValue) * Vector3.up * m_CircleRadius;
        m_CursorImg.rectTransform.rotation = Quaternion.Euler(0, 0, angle - tot * normalizedValue);

        if (!m_OverloadStyle) return;

        UpdateVisualsByState(m_OverloadStyle.GetState(), normalizedValue);
    }

    protected virtual void UpdateVisualsByState(OverloadWeaponState state, float normalizedValue)
    {
        m_ReloadTextGO.SetActive(false);
        
        switch (state)
        {
            case OverloadWeaponState.CanShoot:
                m_ReloadTextGO?.SetActive(false);
                m_FillImg.color = m_ShootColor;
                break;

            case OverloadWeaponState.DefaultCool:
                m_ReloadTextGO?.SetActive(false);
                m_FillImg.color = m_ReloadColor;
                break;

            case OverloadWeaponState.CoolBuffed:
                m_ReloadTextGO?.SetActive(false);
                m_FillImg.color = m_OverloadBuffColor;
                break;

            case OverloadWeaponState.CoolNerfed:
                m_ReloadTextGO?.SetActive(false);

                m_FillImg.color = m_OverloadNerfColor;
                break;

            case OverloadWeaponState.OverloadCool:
                m_ReloadTextGO?.SetActive(true);

                m_FillImg.color = m_ReloadColor;
                break;
        }
    }

    private void UpdateUI()
    {
        if (!m_Player.Get().GetPlayerCombat().GetSecondaryCombatStyle())
        {
            m_BackgroundImg.fillAmount = m_WithoutManaFA;
        }
        else
        {
            m_BackgroundImg.fillAmount = m_WithManaFA;
        }

        SetupFills();
    }

    protected virtual void SetupFills()
    {
        if (!m_OverloadStyle) return;

        Vector2 buffValues = m_OverloadStyle.GetRangeToBuff();
        Vector2 resetValues = m_OverloadStyle.GetRangeToReset();

        float normalizedValue = Mathf.Clamp(m_OverloadStyle.GetCurrentTemperature(), 0, 100) / 100;
        float tot = 360 * m_BackgroundImg.fillAmount;
        float angle = m_BackgroundImg.rectTransform.eulerAngles.z;

        m_FillImg.rectTransform.eulerAngles = new Vector3(0, 0, angle);
        m_FillImg.fillAmount = 0;

        m_BlueImg.rectTransform.eulerAngles = new Vector3(0, 0, angle - tot * (resetValues.x * .01f));
        m_BlueImg.fillAmount = (resetValues.y - resetValues.x) * .01f * m_BackgroundImg.fillAmount;

        m_YellowImg.rectTransform.eulerAngles = new Vector3(0, 0, angle - tot * (buffValues.x * .01f));
        m_YellowImg.fillAmount = (buffValues.y - buffValues.x) * .01f * m_BackgroundImg.fillAmount;

        m_CursorImg.rectTransform.anchoredPosition = Quaternion.Euler(0, 0, angle - tot * normalizedValue) * Vector3.up * m_CircleRadius;
        m_CursorImg.rectTransform.rotation = Quaternion.Euler(0, 0, angle - tot * normalizedValue);

        if (m_OverloadStyle.GetState() != OverloadWeaponState.Overload) DisableReloadSkills();
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

    protected virtual void OnDestroy()
    {
        Unbind();
    }
}