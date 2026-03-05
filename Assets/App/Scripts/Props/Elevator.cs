using DG.Tweening;
using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityEvent onDestinationReached;

    float m_OffsetY;
    PlayerController m_Player;

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        if (m_ColliderDetector != null) m_ColliderDetector.OnTriggerEnterCallback += OnEnter;
    }

    private void Start()
    {
        m_State = m_StartingState;
        if (m_State == ElevatorState.Top) m_Movable.transform.position = m_TopPos.position;
        else m_Movable.transform.position = m_ButtomPos.position;
    }

    public void OnEnter(Collider collid)
    {
        if (!m_HasMoved && collid.CompareTag("Player"))
        {
            m_HasMoved = true;
            m_LockCollids?.SetActive(true);

            m_Player = collid.GetComponent<PlayerController>();
            m_OffsetY = m_Player.GetRigidbody().position.y - m_Movable.position.y;

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
        m_Player.transform.DOMoveY(m_TopPos.position.y + m_OffsetY, m_MovementTime).SetEase(m_MovementEase);
        m_Movable.DOMove(m_TopPos.position, m_MovementTime).SetEase(m_MovementEase).OnComplete(() =>
        {
            m_IsMoving = false;
            m_State = ElevatorState.Top;
            m_Player = null;
            onDestinationReached?.Invoke();
            m_LockCollids?.SetActive(false);
        });
    }

    void MoveToButtom()
    {
        m_IsMoving = true;

        m_Movable.DOKill();
        m_Player.transform.DOMoveY(m_ButtomPos.position.y + m_OffsetY, m_MovementTime).SetEase(m_MovementEase);
        m_Movable.DOMove(m_ButtomPos.position, m_MovementTime).SetEase(m_MovementEase).OnComplete(() =>
        {
            m_IsMoving = false;
            m_State = ElevatorState.Buttom;
            onDestinationReached?.Invoke();
        });
    }
}