using UnityEngine;
using UnityEngine.Serialization;

public class EntityRotationVisual : MonoBehaviour
{
    [FormerlySerializedAs("rotationTime")]
    [Header("Settings")]
    [SerializeField] private float m_RotationTime;

    [FormerlySerializedAs("maxRotationAngle")] [SerializeField] private float m_MaxRotationAngle;

    [FormerlySerializedAs("rotationPivot")]
    [Header("References")]
    [SerializeField] private Transform m_RotationPivot;

    private Vector3 m_CurrentRot;
    private Vector3 m_RotationVelocity;

    //[Header("Input")]
    //[Header("Output")]

    public void Rotate(Vector3 input)
    {
        Vector3 target = new Vector3(input.z, 0, -input.x) * m_MaxRotationAngle;

        m_CurrentRot = Vector3.SmoothDamp(m_CurrentRot, target, ref m_RotationVelocity, m_RotationTime);
        m_RotationPivot.localEulerAngles = m_CurrentRot;
    }
}