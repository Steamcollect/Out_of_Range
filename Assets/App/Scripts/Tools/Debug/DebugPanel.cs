using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject m_Content;

    [Header("References")]
    [SerializeField] InputActionReference m_ToggleIA;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        m_ToggleIA.action.Enable();
        m_Content.SetActive(false);
    }

    private void OnEnable()
    {
        m_ToggleIA.action.started += Toggle;
    }

    private void OnDisable()
    {
        m_ToggleIA.action.started -= Toggle;
    }

    void Toggle(InputAction.CallbackContext ctx)
    {
        m_Content.SetActive(!m_Content.activeSelf);
    }
}