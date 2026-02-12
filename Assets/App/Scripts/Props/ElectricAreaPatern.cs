using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ElectricAreaPatern : MonoBehaviour
{
    [SerializeField] int m_CurrentIndex = 0;
    [SerializeField] ElectricPatern[] m_Parterns;

    [System.Serializable]
    struct ElectricPatern
    {
        public ElectricArea[] Areas;
        public float WaitingTime;
    }

    private void Start()
    {
        ResetAreas();
    }

    [Button]
    public void HandlePatern()
    {
        StartCoroutine(HandlePaterns());
    }

    IEnumerator HandlePaterns()
    {
        foreach (var area in m_Parterns[m_CurrentIndex].Areas)
        {
            area.HandleLoop();
        }

        yield return new WaitForSeconds(m_Parterns[m_CurrentIndex].WaitingTime);

        m_CurrentIndex = (m_CurrentIndex + 1) %  m_Parterns.Length;

        StartCoroutine(HandlePaterns());
    }

    public void ResetAreas()
    {
        for (int i = 0; i < m_Parterns.Length; i++)
        {
            for (int j = 0; j < m_Parterns[i].Areas.Length; j++)
            {
                m_Parterns[i].Areas[j].SetAsSafe();
            }
        }
    }
}