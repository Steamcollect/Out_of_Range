using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [SerializeField] GameObject m_Target;
    [SerializeField] Transform m_Pivot;

    private void Update()
    {
        m_Pivot.LookAt(m_Target.transform.position);
    }
}
