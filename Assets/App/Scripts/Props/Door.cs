using System.Linq;
using DG.Tweening;
using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_OpenTime;
    [SerializeField] float m_DelayBeforeOpen = .5f;

    [SerializeField] DoorStartType m_Type;
    public enum DoorStartType { Open, Close, Default }

    [SerializeField, ReadOnly] int m_DetectionCount = 0;
    [SerializeField, ReadOnly] bool m_IsLock = true;

    Vector3 m_ClosePos;
    Vector3 m_OpenPos;

    [Header("References")]
    [SerializeField] GameObject m_DoorMesh;
    [SerializeField] Transform m_OpenPoint;
    [SerializeField] Transform m_ClosePoint;

    EntityController currentEntity;

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
    }

    private void Start()
    {
        if (m_Type == DoorStartType.Open) OpenDoor();
        else CloseDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        m_DetectionCount++;

        if(!other.gameObject.CompareTag("Player") && other.gameObject.TryGetComponent(out EntityController entity))
            entity.OnDeath += ConnectEntityOnDeath;

        if (!m_IsLock)
        {
            Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_DetectionCount--;

        if (!other.gameObject.CompareTag("Player") && other.gameObject.TryGetComponent(out EntityController entity))
            entity.OnDeath -= ConnectEntityOnDeath;

        if (!m_IsLock && m_DetectionCount <= 0)
        {
            m_DetectionCount = 0;
            Close();
        }
    }

    public void OpenDoor()
    {
        m_IsLock = false;

        if (m_DetectionCount > 0)
        {
            this.Delay(() =>
            {
                Open();
            }, m_DelayBeforeOpen);
        }

        foreach (MeshMatChanger mc in m_MeshChanger)
            mc.ChangeMat(m_OpenedMat);
    }

    public void CloseDoor()
    {
        m_IsLock = true;
        if (m_DetectionCount > 0) Close();

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

    void ConnectEntityOnDeath(EntityController entity)
    {
        entity.OnDeath -= ConnectEntityOnDeath;

        m_DetectionCount--;
        if (!m_IsLock && m_DetectionCount <= 0)
        {
            m_DetectionCount = 0;
            Close();
        }
    }
}