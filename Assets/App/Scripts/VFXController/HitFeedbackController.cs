using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

public class HitFeedbackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EntityHealth m_EntityHealth;

    [FormerlySerializedAs("m_HitFlickerFeedback")] [SerializeField]
    private MMF_Player m_HitFeedback;

    private void OnEnable()
    {
        m_EntityHealth.OnTakeDamage += m_HitFeedback.PlayFeedbacks;
    }

    private void OnDisable()
    {
        m_EntityHealth.OnTakeDamage -= m_HitFeedback.PlayFeedbacks;
    }
}