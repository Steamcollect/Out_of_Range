using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatStyleHUD : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] Image fillImg;

    [Header("Input")]
    [SerializeField] RSO_PlayerController playerController;

    //[Header("Output")]

    private void Start()
    {
        playerController.Get().GetCombat().GetCombatStyle().OnAmmoChange += SetFillValue;
    }

    public void SetFillValue(float value,float max)
    {
        fillImg.fillAmount = Mathf.Clamp(value, 0, max) / max;
    }
}