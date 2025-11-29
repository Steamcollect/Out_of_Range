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

    public void SetFillValue(int value, int max)
    {
        fillImg.fillAmount = (float)Mathf.Clamp(value, 0, max) / max;
    }
}