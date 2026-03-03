using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using UnityEngine;

public class EnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField, ReadOnly] protected EnemyStates m_CurrentState;

    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public void OnSpawn()
    {
        m_CurrentState = EnemyStates.Chasing;

        IsSpawning = true;
        m_Health.GainInvincibility(m_SpawnDuration);

        this.Delay(() => {
            IsSpawning = false;
        }, m_SpawnDuration);
    }
    public void SetAware()
    {
        m_CurrentState = EnemyStates.Chasing;
    }
}