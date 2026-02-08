using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_PowerUp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject m_PowerUpPrefab;
        
    [Header("References")]
    [SerializeField] private RSO_CurrentPowerUp m_CurrentPowerUp;
    [SerializeField] private GameObject m_PowerUpContainer;

    private readonly List<UI_PowerUpSlot> m_PowerUpSlots = new(capacity:3);
        
    private void OnEnable() => m_CurrentPowerUp.Value.OnListChanged += UpdateUI;
    private void OnDisable() => m_CurrentPowerUp.Value.OnListChanged -= UpdateUI;
    private void Start() => UpdateUI(m_CurrentPowerUp.Value.GetPowerUps());

    private void UpdateUI(List<SSO_PowerUp> powerUps)
    { 
        HashSet<SSO_PowerUp> currentPowerUps = m_PowerUpSlots.Select(slot => slot.PowerUp).ToHashSet();
                
        List<SSO_PowerUp> powerUpsToAdd = powerUps.Except(currentPowerUps).ToList();
        List<SSO_PowerUp> powerUpsToRemove = currentPowerUps.Except(powerUps).ToList();
                
        foreach (var powerUpToRemove in powerUpsToRemove)
        {
            var slotToRemove = m_PowerUpSlots.FirstOrDefault(slot => slot.PowerUp == powerUpToRemove);
            if (slotToRemove)
            {
                m_PowerUpSlots.Remove(slotToRemove);
                slotToRemove.PlayDisappearAnimation();
                Destroy(slotToRemove.gameObject, 0.5f); 
            }
        }
                
        foreach (var powerUp in powerUpsToAdd)
        {
            GameObject powerUpObj = Instantiate(m_PowerUpPrefab, m_PowerUpContainer.transform);
            UI_PowerUpSlot powerUpSlot = powerUpObj.GetComponent<UI_PowerUpSlot>();
            powerUpSlot.Setup(powerUp);
            m_PowerUpSlots.Add(powerUpSlot);
        }
                
        for (int i = 0; i < m_PowerUpSlots.Count; i++)
        {
            m_PowerUpSlots[i].transform.rotation = i % 2 == 0 ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
        }                
    }
        
}