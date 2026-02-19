using DG.Tweening;
using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_MovementTime;
    [SerializeField, EnumButtons] ElevatorState m_StartingState; 
    [SerializeField, ReadOnly] ElevatorState m_State;
    [SerializeField] Ease m_MovementEase;

    bool m_IsMoving = false;
    bool m_HasMoved = false;

    enum ElevatorState { Top, Buttom}

    [Header("References")]
    [SerializeField] Transform m_Movable;
    [SerializeField] Transform m_ButtomPos, m_TopPos;
    [SerializeField] GameObject m_LockCollids;

    [Space(8)]
    [SerializeField] ColliderCallback m_ColliderDetector;

    Transform m_PlayerCollided;

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        m_ColliderDetector.OnTriggerEnterCallback += OnEnter;
    }

    private void Start()
    {
        m_State = m_StartingState;
        if (m_State == ElevatorState.Top) m_Movable.transform.position = m_TopPos.position;
        else m_Movable.transform.position = m_ButtomPos.position;
    }

    private void OnEnter(Collider collid)
    {
        if (!m_HasMoved && collid.CompareTag("Player"))
        {
            m_HasMoved = true;
            m_LockCollids.SetActive(true);

            m_PlayerCollided = collid.transform;
            m_PlayerCollided.SetParent(m_Movable);

            if (!m_IsMoving)
            {
                this.Delay(() =>
                {
                    if (m_State == ElevatorState.Top) MoveToButtom();
                    else MoveToTop();
                }, 1);                
            }
        }
    }

    void MoveToTop()
    {
        m_IsMoving = true;
        
        m_Movable.DOKill();
        m_Movable.DOMove(m_TopPos.position, m_MovementTime).SetEase(m_MovementEase).OnComplete(() =>
        {
            m_IsMoving = false;
            m_State = ElevatorState.Top;
            m_PlayerCollided.SetParent(null);
        });
    }

    void MoveToButtom()
    {
        m_IsMoving = true;

        m_Movable.DOKill();
        m_Movable.DOMove(m_ButtomPos.position, m_MovementTime).SetEase(m_MovementEase).OnComplete(() =>
        {
            m_IsMoving = false;
            m_State = ElevatorState.Buttom;
        });
    }
}