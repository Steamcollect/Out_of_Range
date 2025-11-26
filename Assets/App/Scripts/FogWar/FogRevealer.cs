using System;
using FischlWorks_FogWar;
using UnityEngine;

public class FogRevealer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private FogWarManager.FogRevealer m_FogRevealer;

    private void OnEnable()
    {
        if (!FogWarManager.HasInstance()) return;
        FogWarManager.Instance.AddFogRevealer(m_FogRevealer);
    }

    private void OnDisable()
    {
        if (!FogWarManager.HasInstance()) return;
        FogWarManager.Instance.RemoveFogRevealer(m_FogRevealer);
    }
}
