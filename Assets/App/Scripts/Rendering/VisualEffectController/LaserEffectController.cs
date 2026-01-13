using UnityEngine;
using UnityEngine.VFX;

public class LaserEffectController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect m_VisualEffect;
    [SerializeField] private TurretEnemyController m_TurretEnemyController;
    [SerializeField] private RayRangeEnemyCombat m_RangeEnemyCombat;

    private void OnEnable()
    {
        m_TurretEnemyController.OnStateChanged += OnStateChanged;
        m_RangeEnemyCombat.OnShootLaunched += Shoot;
        m_RangeEnemyCombat.OnShootCompleted += TargetEnemy;
    }

    private void OnDisable()
    {
        m_TurretEnemyController.OnStateChanged -= OnStateChanged;
        m_RangeEnemyCombat.OnShootLaunched -= Shoot;
        m_RangeEnemyCombat.OnShootCompleted -= TargetEnemy;
    }

    private void Start()
    {
        Disable();
    }

    private void OnStateChanged(EnemyStates state)
    {
        if (state == EnemyStates.Chasing)
        {
            TargetEnemy();
        }
        else if(state == EnemyStates.Idle)
        {
            Disable();
        }
    }

    private void Shoot(float delayBeforeShoot, float delayAfterShoot)
    {
        m_VisualEffect.Reinit();
        m_VisualEffect.SetFloat("DelayBeforeShoot", delayBeforeShoot);
        m_VisualEffect.SetFloat("DelayAfterShoot", delayAfterShoot);
        m_VisualEffect.SetBool("TrailEnabled", false);
        m_VisualEffect.SendEvent("Shoot");
    }

    private void TargetEnemy()
    {
        m_VisualEffect.Reinit();
        m_VisualEffect.SetBool("TrailEnabled", true);
        m_VisualEffect.SendEvent("Target");
    }

    private void Disable()
    {
        m_VisualEffect.Reinit();
    }
}