using System.Linq;
using MVsToolkit.Dev;
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
}

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