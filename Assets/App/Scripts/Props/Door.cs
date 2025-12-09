using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    public enum DoorStartType
    {
        Open,
        Close,
        Default
    }

    [Header("Settings")]
    [SerializeField] private float m_OpenTime;
    [SerializeField] private DoorStartType m_Type;

    [Header("References")]
    [SerializeField] private Transform m_OpenPoint;
    [SerializeField] private Transform m_ClosePoint;
    
    private Vector3 m_ClosePos;
    private Vector3 m_OpenPos;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        m_OpenPos = m_OpenPoint.position;
        m_ClosePos = m_ClosePoint.position;

        switch (m_Type)
        {
            case DoorStartType.Open:
                transform.position = m_OpenPos;
                break;
            case DoorStartType.Close:
                transform.position = m_ClosePos;
                break;
        }
    }

    public void OpenDoor()
    {
        transform.DOKill();
        transform.DOMove(m_OpenPos, m_OpenTime);
    }

    public void CloseDoor()
    {
        transform.DOKill();
        transform.DOMove(m_ClosePos, m_OpenTime);
    }
}