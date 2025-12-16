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

    [Header("References")]
    [SerializeField] Image m_FillImg;
    [SerializeField] TMP_Text m_ReloadTxt;
    [SerializeField] RSO_PlayerCameraController m_PlayerCameraController;

    RangeOverload_CombatStyle m_OverloadStyle;

    [Space(10)]
    [SerializeField] RectTransform m_ParentRect;
    [SerializeField] RectTransform m_BuffZoneRct;
    [SerializeField] RectTransform m_NerfZoneRct;

    [Header("Input")]
    [SerializeField] RSO_PlayerController m_PlayerController;

    //[Header("Output")]

    private void Start()
    {
        PlayerCombat combat = m_PlayerController.Get().GetCombat() as PlayerCombat;
        m_OverloadStyle = combat.GetPrimaryCombatStyle() as RangeOverload_CombatStyle;

        m_OverloadStyle.OnAmmoChange += SetFillValue;

        Vector2 buffValues = m_OverloadStyle.RangeToBuff;
        Vector2 resetValues = m_OverloadStyle.RangeToReset;
        float parentWidth = m_ParentRect.rect.width;


    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    public void SetFillValue(float value, float max)
    {
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

        m_FillImg.fillAmount = Mathf.Clamp(value, 0, max) / max;
    }

    void UpdatePosition()
    {
        transform.position = (Vector2)m_PlayerCameraController.Get().GetCamera()
            .WorldToScreenPoint(m_PlayerController.Get().GetTargetPosition()) + m_PosOffset;
    }
}