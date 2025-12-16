using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ProjectVersionTxtDebug : MonoBehaviour
{
    [Title("References")]
    [SerializeField] TMP_Text m_DebugTxt;

    void OnEnable()
    {
        if (m_DebugTxt == null) return;

        m_DebugTxt.text = $"version : {Application.version}";
    }
}