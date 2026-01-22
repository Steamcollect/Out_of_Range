using UnityEngine;
using MVsToolkit.Dev;

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
    }
    public void SetAware()
    {
        m_CurrentState = EnemyStates.Chasing;
    }
}