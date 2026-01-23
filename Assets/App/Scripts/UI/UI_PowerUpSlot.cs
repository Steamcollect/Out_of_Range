using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerUpSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image m_IconImage;
    [Space] 
    [SerializeField] private Animator m_Animator;

    private SSO_PowerUp m_PowerUp;
    
    public SSO_PowerUp PowerUp => m_PowerUp;
    
    public void Setup(SSO_PowerUp powerUp)
    {
        m_PowerUp = powerUp;
        
        m_IconImage.sprite = powerUp.Icon;
        m_IconImage.color = powerUp.ColorIcon;
    }

    private void OnEnable()
    {
        m_Animator.Play("Appear");
    }
    
    public void PlayDisappearAnimation()
    {
        m_Animator.Play("Disappear");
    }
}
