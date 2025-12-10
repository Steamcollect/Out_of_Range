using System;
using UnityEngine;

public class SaveManager : RegularSingleton<SaveManager>
{
    [Header("Setting")]
    [SerializeField] private bool m_AutoRegularSave = true;
    [SerializeField, Tooltip("Time in second")] private float m_AutoRegularSaveTime = 300f;
    
    private float m_Timer;
    
    
    private void Update()
    {
        if (!m_AutoRegularSave) 
            return;

        m_Timer += Time.deltaTime;
        if (!(m_Timer >= m_AutoRegularSaveTime)) return;
        m_Timer = 0f;
        SavingSystem.Save();
    }
    
    void OnApplicationQuit() => SavingSystem.Save();
    private void OnDestroy() => SavingSystem.Save();
}
