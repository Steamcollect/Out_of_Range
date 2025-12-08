using UnityEngine;

public class EnemyState : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] EnemyStates currentState = EnemyStates.Idle;

    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public void SetState(EnemyStates newState) => currentState = newState;
    public EnemyStates GetState() => currentState;
}

public enum EnemyStates
{
    Idle,
    Aware,
    Chasing,
}