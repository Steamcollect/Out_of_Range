using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] Image fillImg;

    [Header("Input")]
    [SerializeField] RSO_PlayerController playerController;

    //[Header("Output")]

    private void Start()
    {
        EntityHealth health = playerController.Get().GetHealth();
        health.OnTakeDamage += ()=>
        {
            SetFillValue(health.GetCurrentHealth(), health.GetMaxHealth());
        };
        
        health.OnDeath+= ()=>
        {
            SetFillValue(0, health.GetMaxHealth());
        };

        SetFillValue(health.GetCurrentHealth(), health.GetMaxHealth());
    }

    public void SetFillValue(int value, int max)
    {
        fillImg.fillAmount = (float)Mathf.Clamp(value, 0, max) / max;
    }
}