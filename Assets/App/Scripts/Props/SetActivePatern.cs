using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;

public class SetActivePatern : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject[] m_Objetcs;
    [SerializeField, Inline] ActivePatern[] m_Paterns;

    [SerializeField] bool m_StartOnAwake;
    [SerializeField] bool m_Loop;

    [System.Serializable]
    struct ActivePatern
    {
        public float Time;
        public bool SetActive;
    }

    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        if (m_StartOnAwake) Loop();
    }

    public void Loop()
    {
        StartCoroutine(_Loop());
    }

    IEnumerator _Loop()
    {
        foreach (ActivePatern patern in m_Paterns)
        {
            yield return new WaitForSeconds(patern.Time);
            foreach (GameObject obj in m_Objetcs)
            {
                obj.SetActive(patern.SetActive);
            }
        }

        if (m_Loop) StartCoroutine(_Loop());
    }
}