using DG.Tweening;
using UnityEngine;

public class PlayerClone : MonoBehaviour
{
    [SerializeField] Transform m_HeadPivot, m_ArmsPivot;
    [SerializeField] MeshRenderer[] m_Renderers;
    Material mat;

    public void Init()
    {
        mat = new Material(m_Renderers[0].material);

        foreach (var renderer in m_Renderers)
        {
            renderer.materials = new Material[] { mat };
        }
    }

    public void SetPivots(Vector3 headPos, Vector3 armsPos)
    {
        m_HeadPivot.localPosition = headPos;
        m_ArmsPivot.localPosition = armsPos;
    }

    public void SetRotations(Quaternion headRot, Quaternion armsRot)
    {
        m_HeadPivot.localRotation = headRot;
        m_ArmsPivot.localRotation = armsRot;
    }

    public void Fade(float time)
    {
        mat.DOFade(1, 0).OnComplete(() =>
        {
            mat.DOFade(0, time).OnComplete(() => { gameObject.SetActive(false); });
        });
    }
}