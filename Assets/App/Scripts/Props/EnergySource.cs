using System;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : MonoBehaviour
{
    [SerializeField] private List<EntityHealth> linkedObjects;
    private List<LineRenderer> m_LineRenderers;
    public Material lineMaterial;
    void Start()
    {
        m_LineRenderers = new List<LineRenderer>();
        
        foreach (EntityHealth linkedObject in linkedObjects)
        {
            linkedObject.GainInvincibility();
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            m_LineRenderers.Add(lr);
            lr.positionCount = 2;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f; 
            lr.material = lineMaterial;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, linkedObject.transform.position);
        }
    }

    private void Update()
    {
        for (int i = 0; i < linkedObjects.Count; i++)
        {
            if (linkedObjects[i] != null)
            {
                m_LineRenderers[i].SetPosition(0, transform.position);
                m_LineRenderers[i].SetPosition(1, linkedObjects[i].transform.position);
            }
        }
    }
}
