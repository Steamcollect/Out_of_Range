using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_PowerUp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] UI_PowerUpSlot m_PowerUpSlotPrefab;

    [Header("Animation Settings")]
    [SerializeField] float m_DelayBetweenWaves;
    [SerializeField] float m_DelayBetweenElements;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerPowerUpChange m_OnPowerUpChange;

    List<UI_PowerUpSlot> m_PowerUpSlots = new(capacity:3);
        
    private void OnEnable() => m_OnPowerUpChange.Action += UpdateUI;
    private void OnDisable() => m_OnPowerUpChange.Action -= UpdateUI;

    private void Start()
    {
        StartCoroutine(Animation());
    }

    private void UpdateUI(List<PowerUpHandler> powerUps)
    {
        StopAllCoroutines();
        
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

        List<UI_PowerUpSlot> toRemove = new();

        foreach (UI_PowerUpSlot slot in m_PowerUpSlots)
        {
            if (!powerUps.Contains(slot.GetPowerUp()))
                toRemove.Add(slot);
        }

        foreach (UI_PowerUpSlot slot in toRemove)
        {
            m_PowerUpSlots.Remove(slot);
            slot.DestoyUI();
        }

        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        yield return new WaitUntil(() => m_PowerUpSlots.Count > 0);

        foreach (var item in m_PowerUpSlots)
        {
            item.Bump();
            yield return new WaitForSeconds(m_DelayBetweenElements);
        }

        yield return new WaitForSeconds(m_DelayBetweenWaves);
        StartCoroutine(Animation());
    }
}