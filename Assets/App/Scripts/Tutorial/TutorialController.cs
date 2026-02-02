using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_DelayBetweenPlay = 2f;
    [SerializeField] private bool m_AutoPlay = false;
    [SerializeField] private DemoState m_DemoState = DemoState.Reset;
    
    [Header("References")]
    [SerializeField] private FakeOverloadCombatStyle m_FakeOverloadStyle;
    
    private Coroutine m_CoroutineHandle;

    #region Unity Callbacks

    private void OnEnable()
    {
        if (m_AutoPlay) HandlePlayDemo();
    }
    
    private void OnDisable()
    {
        HandleStopDemo();
    }

    #endregion

    #region Handle API

    public void HandlePlayDemo()
    {
        Action handleSimulation = m_DemoState switch
        {
            DemoState.Reset => m_FakeOverloadStyle.SimulateReset,
            DemoState.Buff => m_FakeOverloadStyle.SimulateBuff,
            DemoState.Nerf => m_FakeOverloadStyle.SimulateNerf,
            DemoState.FullOverload => m_FakeOverloadStyle.SimulateFullOverload,
            _ => null
        };
        m_CoroutineHandle = StartCoroutine(Simulation(handleSimulation));
    }
    
    private IEnumerator Simulation(Action handleSimulation)
    {
        do
        { 
            handleSimulation?.Invoke();
            yield return m_FakeOverloadStyle.CurrentSimulation;
            yield return new WaitForSeconds(m_DelayBetweenPlay);
            m_FakeOverloadStyle?.ResetState();
            
        } while (m_AutoPlay);
    }
    
    public void HandleStopDemo()
    {
        if (m_CoroutineHandle != null)
        {
            StopCoroutine(m_CoroutineHandle);
            m_CoroutineHandle = null;
        }
        m_FakeOverloadStyle?.ResetState();
    }

    #endregion
    
    
    private enum DemoState
    {
        Reset,
        Buff,
        Nerf,
        FullOverload
    }
}

