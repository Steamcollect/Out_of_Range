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
    [SerializeField] private GameObject m_DoorMesh;
    [SerializeField] private Transform m_OpenPoint;
    [SerializeField] private Transform m_ClosePoint;
    
    private Vector3 m_ClosePos;
    private Vector3 m_OpenPos;

    private void Awake()
    {
        m_OpenPos =  m_ClosePoint.position;
        m_ClosePos = m_OpenPoint.position;
        
        switch (m_Type)
        {
            case DoorStartType.Open:
                m_DoorMesh.transform.position = m_OpenPos;
                break;
            case DoorStartType.Close:
                m_DoorMesh.transform.position = m_ClosePos;
                break;
        }
    }

    public void OpenDoor()
    {
        m_DoorMesh.transform.DOKill();
        m_DoorMesh.transform.DOMove(m_OpenPos, m_OpenTime);
    }

    public void CloseDoor()
    {
        m_DoorMesh.transform.DOKill();
        m_DoorMesh.transform.DOMove(m_ClosePos, m_OpenTime);
    }
}