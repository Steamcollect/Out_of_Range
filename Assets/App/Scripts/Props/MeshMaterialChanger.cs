using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshMaterialChanger : MonoBehaviour
{
    [SerializeField] MeshMatChanger[] m_MeshsChanger;
    [SerializeField] Material m_BlueMat, m_RedMat;

    [Space(10)]
    [SerializeField] bool m_ChangeToBlueOnPlayerDetection = false;
    [SerializeField, ShowIf("m_ChangeToBlueOnPlayerDetection", true)] RSO_PlayerController m_Player;
    [SerializeField, ShowIf("m_ChangeToBlueOnPlayerDetection", true)] float m_PlayerDetectionRadius;

    bool m_IsBlue = false;

    private void Update()
    {
        if(m_ChangeToBlueOnPlayerDetection)
        {
            if(!m_IsBlue && Vector3.Distance(transform.position, m_Player.Value.transform.position) < m_PlayerDetectionRadius)
            {
                m_IsBlue = true;
                ChangeToBlue();
            }
            else if(m_IsBlue && Vector3.Distance(transform.position, m_Player.Value.transform.position) > m_PlayerDetectionRadius)
            {
                m_IsBlue = false;
                ChangeToRed();
            }
        }
    }

    public void ChangeToBlue()
    {
        foreach (var item in m_MeshsChanger)
        {
            item.ChangeMat(m_BlueMat);
        }
    }
    
    public void ChangeToRed()
    {
        foreach (var item in m_MeshsChanger)
        {
            item.ChangeMat(m_RedMat);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!m_ChangeToBlueOnPlayerDetection) return;
        Gizmos.DrawWireSphere(transform.position, m_PlayerDetectionRadius);
    }

    [Button]
    public void SetMeshs()
    {
        m_MeshsChanger = GetComponentsInChildren<MeshRenderer>()
            .Select(mesh => new MeshMatChanger { Mesh = mesh, MatIndex = 0 })
            .ToArray();
    }
}

[System.Serializable]
struct MeshMatChanger
{
    public int MatIndex;
    public MeshRenderer Mesh;

    private List<Material> m_MaterialsAssociated;
    
    public void ChangeMat(Material mat)
    {
        m_MaterialsAssociated ??= new List<Material>(capacity: 3);
        Mesh.GetMaterials(m_MaterialsAssociated);
        m_MaterialsAssociated[MatIndex] = mat;
        Mesh.SetMaterials(m_MaterialsAssociated);
    }
}