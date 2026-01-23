using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider m_DashSlider;
    [Space]
    [SerializeField] private RSO_PlayerController m_Controller;
    
    private void OnEnable() => m_Controller.Get().GetDash().OnDash += UpdateUI;
    private void OnDisable() => m_Controller.Get().GetDash().OnDash -= UpdateUI;
    
    private void UpdateUI(float dashTime, float dashCooldown)
    {
        m_DashSlider.DOValue(0f, dashTime).SetEase(Ease.OutSine).OnComplete(() =>
        {
            m_DashSlider.DOValue(1f, dashCooldown - dashTime).SetEase(Ease.InSine);
        });
    }
}
