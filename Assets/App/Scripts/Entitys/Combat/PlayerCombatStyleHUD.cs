using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerCombatStyleHUD : MonoBehaviour
{
    //[Header("Settings")]
    [FormerlySerializedAs("fillImg")]
    [Header("References")]
    [SerializeField] private Image m_FillImg;

    [FormerlySerializedAs("playerController")]
    [Header("Input")]
    [SerializeField] private RSO_PlayerController m_PlayerController;

    //[Header("Output")]

    private void Start()
    {
        m_PlayerController.Get().GetCombat().GetCombatStyle().OnAmmoChange += SetFillValue;
    }

    public void SetFillValue(float value, float max)
    {
        m_FillImg.fillAmount = Mathf.Clamp(value, 0, max) / max;
    }
}