using System;
using System.Threading.Tasks;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

public class UI_Health : MonoBehaviour
{
   [Header("Settings")]
   [SerializeField] private Vector2 m_PosOffset;
   [SerializeField] private float m_VisibilityDuration = 2f;
   [SerializeField] private float m_FadeDuration = 0.5f;
   [SerializeField] private float m_DelayBetweenHealthBarUpdate = 0.1f;
   [SerializeField] private CanvasGroup m_CanvasGroup;
   
   [Header("References")]
   [SerializeField] private GameObject m_Container;
   [SerializeField] private MMProgressBar m_HealthBarSlotPrefab;
   [Space]
   [SerializeField] private RSO_PlayerController m_PlayerController;
   [SerializeField] private RSO_PlayerCameraController m_PlayerCameraController;

   [Header("Output")]
   [SerializeField] private UnityEvent m_OnUiUpdateStart;
   [SerializeField] private UnityEvent m_OnUiUpdateEnd;
   
   private MMProgressBar[] m_HealthBarBuffer;
   private float m_LastHealthPoint = 0;
   
   private void OnEnable()
   {
      m_PlayerController.Get().GetHealth().OnTakeDamage += UpdateUI;
      m_PlayerController.Get().GetHealth().OnHeal += UpdateUI;
      SetupDependency();
      FillContainerHealthSlot();
   }
   
   private void OnDisable()
   {
      ClearContainerHealthSlot();
      m_PlayerController.Get().GetHealth().OnTakeDamage -= UpdateUI;
      m_PlayerController.Get().GetHealth().OnHeal -= UpdateUI;
   }

   private void FillContainerHealthSlot()
   {
      m_HealthBarBuffer = new MMProgressBar[Mathf.RoundToInt(m_PlayerController.Get().GetHealth().GetMaxHealth())];
      for (int i = 0; i < m_PlayerController.Get().GetHealth().GetMaxHealth(); i++)
      {
         MMProgressBar healthBar = Instantiate(m_HealthBarSlotPrefab, m_Container.transform);
         healthBar.UpdateBar(1, 0, 1);
         m_HealthBarBuffer[i] = healthBar;
      }
   }

   private void SetupDependency()
   {
      m_LastHealthPoint = m_PlayerController.Get().GetHealth().GetMaxHealth();
      m_CanvasGroup.alpha = 0f;
   }
   
   private void CleanUpDependency()
   {
      m_CanvasGroup.DOKill();
   }
   
   private void ClearContainerHealthSlot()
   {
      if (m_HealthBarBuffer == null) return;
      for (int i = 0; i < m_HealthBarBuffer.Length; i++)
      {
         if (m_HealthBarBuffer[i]) Destroy(m_HealthBarBuffer[i].gameObject);
      }
      Array.Clear(m_HealthBarBuffer, 0, m_HealthBarBuffer.Length);
   }
   
   private async void UpdateUI()
   {
      m_OnUiUpdateStart.Invoke();
      m_CanvasGroup.DOFade(1f, m_FadeDuration).ChangeStartValue(0f);
      
      await Task.Delay(Mathf.RoundToInt(m_FadeDuration * 1000));
      
      int difference = Mathf.Abs(Mathf.RoundToInt(m_LastHealthPoint - m_PlayerController.Get().GetHealth().GetCurrentHealth()));
      int index = Mathf.Clamp(Mathf.RoundToInt(m_LastHealthPoint) - 1, 0, m_HealthBarBuffer.Length - 1);
      
      
      if (m_LastHealthPoint > m_PlayerController.Get().GetHealth().GetCurrentHealth())
      {
        for (int i = 0; i < difference; i++)
        {
           m_HealthBarBuffer[index - i].UpdateBar(0, 0, 1);
           await Task.Delay(Mathf.RoundToInt(m_DelayBetweenHealthBarUpdate * 1000));
        }
      }
      else
      {
         for (int i = 1; i <= difference; i++)
         {
            m_HealthBarBuffer[index + i].UpdateBar(1, 0, 1);
            await Task.Delay(Mathf.RoundToInt(m_DelayBetweenHealthBarUpdate * 1000));
         }
      }
      
      m_LastHealthPoint = m_PlayerController.Get().GetHealth().GetCurrentHealth();
      
      await Task.Delay( Mathf.RoundToInt(m_VisibilityDuration * 1000));
      
      m_CanvasGroup.DOFade(0f, m_FadeDuration).ChangeStartValue(1f);
      await Task.Delay(Mathf.RoundToInt(m_FadeDuration * 1000));
      m_OnUiUpdateEnd.Invoke();
   }

   private void LateUpdate()
   {
      UpdatePosition();
   }

   private void UpdatePosition()
   {
      
      transform.position = (Vector2)m_PlayerCameraController.Get().GetCamera()
         .WorldToScreenPoint(m_PlayerController.Get().GetTargetPosition()) + m_PosOffset;
   }
}

