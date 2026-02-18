using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerUpSlot : MonoBehaviour
{
    [SerializeField] Image m_VisualImg;
    [SerializeField] Image m_TimerImg;

    PowerUpHandler m_PowerUpHandler;

    public void Setup(PowerUpHandler handler)
    {
        m_PowerUpHandler = handler;
        m_VisualImg.sprite = handler.PowerUp.Icon;
        m_TimerImg.fillAmount = 0;

        transform.DOScale(1.1f, .07f).OnComplete(() =>
        {
            transform.DOScale(1, .1f);
        });
    }

    private void Update()
    {
        if (m_PowerUpHandler.timer <= 0)
        {
            m_TimerImg.fillAmount = 1;
        }
        else m_TimerImg.fillAmount = (m_PowerUpHandler.timer / m_PowerUpHandler.MaxTime) * -1 + 1;
    }

    public PowerUpHandler GetPowerUp() => m_PowerUpHandler;
}