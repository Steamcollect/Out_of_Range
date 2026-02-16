using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ElectricAreaPatern : MonoBehaviour
{
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
        for (int i = 0; i < m_Parterns.Length; i++)
        {
            for (int j = 0; j < m_Parterns[i].Areas.Length; j++)
            {
                m_Parterns[i].Areas[j].SetState(ElectricArea.ElectricAreaState.Warning);
                m_Parterns[i].Areas[j].OnSetAsWarning();
                m_Parterns[i].Areas[j].HandleLoop();
            }
            yield return new WaitForSeconds(m_Parterns[i].WaitingTime);
        }
    }

    public void ResetAreas()
    {
        for (int i = 0; i < m_Parterns.Length; i++)
        {
            for (int j = 0; j < m_Parterns[i].Areas.Length; j++)
            {
                m_Parterns[i].Areas[j].OnSetAsSafe();
            }
        }
    }
}