using UnityEngine;

public class BossPowerupVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject m_AttackSpeedPowerUp;
    [SerializeField] private GameObject m_StrengthPowerUp;
    [SerializeField] private GameObject m_ClonePowerUp;

    private void Awake()
    {
        m_AttackSpeedPowerUp.SetActive(false);
        m_StrengthPowerUp.SetActive(false);
        m_ClonePowerUp.SetActive(false);
    }

    public void SetPowerUpVisual(int index)
    {
        switch (index)
        {
            case 0:
                m_AttackSpeedPowerUp.SetActive(true);
                break;
            case 1:
                m_StrengthPowerUp.SetActive(true);
                break;
            case 2:
                m_ClonePowerUp.SetActive(true);
                break;
        }
    }
}