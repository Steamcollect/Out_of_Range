using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class UI_Health : MonoBehaviour
{
   [Header("References")]
   [SerializeField] private MMProgressBar m_HealthBar;
   [SerializeField] private MMProgressBar m_LastPointHealthBar;
   [Space]
   [SerializeField] private RSO_PlayerController m_Controller;
   
   private void OnEnable()
   {
      m_Controller.Get().GetHealth().OnTakeDamage += UpdateUI;
      m_Controller.Get().GetHealth().OnDeath += UpdateUI;
      m_Controller.Get().GetHealth().OnHeal += UpdateUI;
   }
   
   private void OnDisable()
   {
      m_Controller.Get().GetHealth().OnTakeDamage -= UpdateUI;
      m_Controller.Get().GetHealth().OnDeath -= UpdateUI;
      m_Controller.Get().GetHealth().OnHeal -= UpdateUI;
   }

   private void UpdateUI()
   {
      m_HealthBar.UpdateBar(m_Controller.Get().GetHealth().GetCurrentHealth()-1, 0,
         m_Controller.Get().GetHealth().GetMaxHealth()-1);
      if (m_Controller.Get().GetHealth().GetCurrentHealth() <= 0)
      {
         m_LastPointHealthBar.UpdateBar(0, 0,
            m_Controller.Get().GetHealth().GetMaxHealth());
      }
   }
}

