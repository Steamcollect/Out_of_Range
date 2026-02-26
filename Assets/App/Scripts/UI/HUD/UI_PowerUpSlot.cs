using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerUpSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image m_VisualImg;
    [SerializeField] Image m_TimerImg;

    [Header("Animation Settings")]
    [SerializeField] float m_SpawnShakeForce;
    [SerializeField] float m_SpawnShakeTime;

    bool m_IsInSpawnAnim = false;

    PowerUpHandler m_PowerUpHandler;

    public void Setup(PowerUpHandler handler)
    {
        m_PowerUpHandler = handler;
        m_VisualImg.sprite = handler.PowerUp.Icon;
        m_VisualImg.color = handler.PowerUp.ColorIcon;
        m_TimerImg.fillAmount = 0;

        m_IsInSpawnAnim = true;
        transform.DOShakeRotation(m_SpawnShakeTime, Vector3.forward * m_SpawnShakeForce, 20, 1);
        transform.DOScale(1.3f, m_SpawnShakeTime * .4f).OnComplete(() =>
        {
            m_IsInSpawnAnim = false;
            transform.DOScale(1, m_SpawnShakeTime * .6f);
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

    public void Bump()
    {
        if (m_IsInSpawnAnim) return;
        transform.DOScale(1.16f, m_SpawnShakeTime * .4f).OnComplete(() =>
        {
            transform.DOScale(1, m_SpawnShakeTime * .6f);
        });
    }

    public void DestoyUI()
    {
        transform.DOShakeRotation(m_SpawnShakeTime * .4f, Vector3.forward * m_SpawnShakeForce, 20, 1);
        transform.DOScale(1.2f, m_SpawnShakeTime * .2f).OnComplete(() =>
        {
            transform.DOScale(0, m_SpawnShakeTime * .5f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        });
    }

    public PowerUpHandler GetPowerUp() => m_PowerUpHandler;
}