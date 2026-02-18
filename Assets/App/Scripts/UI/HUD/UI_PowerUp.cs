using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_PowerUp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] UI_PowerUpSlot m_PowerUpSlotPrefab;
        
    [Header("Input")]
    [SerializeField] RSE_OnPlayerPowerUpChange m_OnPowerUpChange;

    List<UI_PowerUpSlot> m_PowerUpSlots = new(capacity:3);
        
    private void OnEnable() => m_OnPowerUpChange.Action += UpdateUI;
    private void OnDisable() => m_OnPowerUpChange.Action -= UpdateUI;

    private void UpdateUI(List<PowerUpHandler> powerUps)
    {
        // --- CREATE NEW SLOTS ---
        foreach (PowerUpHandler powerUp in powerUps)
        {
            UI_PowerUpSlot slot = m_PowerUpSlots.FirstOrDefault(c => c.GetPowerUp() == powerUp);

            if (slot == null)
            {
                slot = Instantiate(m_PowerUpSlotPrefab, transform);
                slot.Setup(powerUp);
                m_PowerUpSlots.Add(slot);
            }
        }

        // --- REMOVE OLD SLOTS ---
        List<UI_PowerUpSlot> toRemove = new();

        foreach (UI_PowerUpSlot slot in m_PowerUpSlots)
        {
            if (!powerUps.Contains(slot.GetPowerUp()))
                toRemove.Add(slot);
        }

        foreach (UI_PowerUpSlot slot in toRemove)
        {
            m_PowerUpSlots.Remove(slot);
            Destroy(slot.gameObject);
        }
    }
}