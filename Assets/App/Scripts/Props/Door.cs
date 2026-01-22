using System.Linq;
using DG.Tweening;
using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_OpenTime;
    [SerializeField] float m_DelayBeforeOpen = .5f;

    [SerializeField] DoorStartType m_Type;
    public enum DoorStartType { Open, Close, Default }

    [SerializeField, ReadOnly] bool m_ContainPlayer = false;
    [SerializeField, ReadOnly] bool m_IsLock = true;

    Vector3 m_ClosePos;
    Vector3 m_OpenPos;

    [Header("References")]
    [SerializeField] GameObject m_DoorMesh;
    [SerializeField] Transform m_OpenPoint;
    [SerializeField] Transform m_ClosePoint;

    [Space(10)]
    [SerializeField, Inline] MeshMatChanger[] m_MeshChanger;
    [SerializeField] Material m_LockedMat, m_OpenedMat;

    [System.Serializable]
    struct MeshMatChanger
    {
        public int MatIndex;
        public MeshRenderer Mesh;

        public void ChangeMat(Material mat)
        {
            Material[] mats = Mesh.materials;
            mats[MatIndex] = mat;

            Mesh.SetMaterials(mats.ToList());
        }
    }

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

    private void Start()
    {
        foreach (MeshMatChanger mc in m_MeshChanger)
            mc.ChangeMat(m_LockedMat);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_ContainPlayer = true;

            if (!m_IsLock)
            {
                Open();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_ContainPlayer = false;

            if (!m_IsLock)
            {
                Close();
            }
        }
    }

    public void OpenDoor()
    {
        m_IsLock = false;

        this.Delay(() =>
        {
            if (m_ContainPlayer) Open();
        }, m_DelayBeforeOpen);

        foreach (MeshMatChanger mc in m_MeshChanger)
            mc.ChangeMat(m_OpenedMat);
    }

    public void CloseDoor()
    {
        m_IsLock = true;
        if (m_ContainPlayer) Close();

        foreach (MeshMatChanger mc in m_MeshChanger)
            mc.ChangeMat(m_LockedMat);
    }

    void Open()
    {
        m_DoorMesh.transform.DOKill();
        m_DoorMesh.transform.DOMove(m_OpenPos, m_OpenTime);
    }

    void Close()
    {
        m_DoorMesh.transform.DOKill();
        m_DoorMesh.transform.DOMove(m_ClosePos, m_OpenTime);
    }
}