using System;
using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.VFX;

public class TrailDetectionTurretController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color m_DefaultColor = Color.white;
    [SerializeField] private Color m_DetectionColor = Color.red;
    [SerializeField] private float m_MaxDistanceRaycast = 100f;
    [SerializeField] private LayerMask m_LayerMask;
    
    [Header("References")]
    [SerializeField] private RangeEnemyCombat m_RangeEnemyCombat;
    [SerializeField] private Transform m_TurretPointer;
    [SerializeField] private LineRenderer m_TrailDetectionRenderer;

    private void OnEnable()
    {
        m_RangeEnemyCombat.OnPrepareToShoot += ChangeColorDetection;
        m_TrailDetectionRenderer.startColor = m_DefaultColor;
    }

    private void OnDisable() => m_RangeEnemyCombat.OnPrepareToShoot -= ChangeColorDetection;
    
    private void ChangeColorDetection(float time)
    {
        m_TrailDetectionRenderer.startColor = m_DetectionColor;
        this.Delay(() =>
        {
            m_TrailDetectionRenderer.startColor = m_DefaultColor;
        }, time);
    }

    private void Update()
    {
        m_TrailDetectionRenderer.SetPosition(0, m_TurretPointer.position);
        
        Vector3 forward = transform.forward;
        forward.z = 0;

        Ray ray = new(m_TurretPointer.position, forward);

        if(Physics.Raycast(ray, out RaycastHit hit, m_MaxDistanceRaycast, m_LayerMask))
        {
            m_TrailDetectionRenderer.SetPosition(1, hit.point);
        }
        else
        {
            Vector3 endPoint = m_TurretPointer.position + (m_TurretPointer.forward * m_MaxDistanceRaycast);
            m_TrailDetectionRenderer.SetPosition(1, endPoint);
        }
    }
    
    
}
