using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

/// <summary>
/// Composant UI qui affiche automatiquement l'icône et/ou le label d'une InputAction
/// et se met à jour lorsque le type de périphérique change.
/// </summary>
public class InputActionIconDisplay : MonoBehaviour
{
    private const string k_LogPrefix = "[InputActionIconDisplay]";

    [Title("Settings")]
    [SerializeField] 
    private bool m_AutoUpdateOnDeviceChange = true;
    [SerializeField] 
    private bool m_HideImageIfNoIcon = true;
    [SerializeField] 
    private InputActionReference m_ActionReference;

    [Title("References")]
    [SerializeField] 
    private Image m_IconImage;
    [SerializeField] 
    private TMP_Text m_LabelText;
    [Space]
    [SerializeField] 
    private SSO_InputBindingIconResolver m_IconResolver;
    [SerializeField] 
    private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    private void OnEnable()
    {
        if (m_AutoUpdateOnDeviceChange && m_CurrentInputDeviceType != null)
        {
            m_CurrentInputDeviceType.OnChanged += OnDeviceTypeChanged;
        }

        UpdateDisplay();
    }

    private void OnDisable()
    {
        if (m_CurrentInputDeviceType != null)
        {
            m_CurrentInputDeviceType.OnChanged -= OnDeviceTypeChanged;
        }
    }

    private void OnDeviceTypeChanged(InputDeviceType newType)
    {
        UpdateDisplay();
    }

    /// <summary>
    /// Met à jour l'affichage de l'icône et du label.
    /// </summary>
    [Button("Refresh")]
    public void UpdateDisplay()
    {
        if (m_IconResolver == null || m_ActionReference == null)
        {
            Debug.LogWarning($"{k_LogPrefix} Missing references on {gameObject.name}");
            return;
        }

        var action = m_ActionReference.action;
        if (action == null)
        {
            Debug.LogWarning($"{k_LogPrefix} Action is null for {m_ActionReference.name}");
            return;
        }

        UpdateIcon(action);
        UpdateLabel(action);
    }

    private void UpdateIcon(InputAction action)
    {
        if (m_IconImage == null) return;

        var icon = m_IconResolver.GetIconForAction(action);
        m_IconImage.sprite = icon;

        if (m_HideImageIfNoIcon)
        {
            m_IconImage.gameObject.SetActive(icon != null);
        }
    }

    private void UpdateLabel(InputAction action)
    {
        if (m_LabelText == null) return;

        m_LabelText.text = m_IconResolver.GetDisplayNameForAction(action);
    }

    /// <summary>
    /// Change l'action affichée dynamiquement.
    /// </summary>
    /// <param name="actionReference">La nouvelle référence d'action</param>
    public void SetAction(InputActionReference actionReference)
    {
        m_ActionReference = actionReference;
        UpdateDisplay();
    }

    /// <summary>
    /// Change l'action affichée dynamiquement.
    /// </summary>
    /// <param name="action">La nouvelle action</param>
    public void SetAction(InputAction action)
    {
        if (m_IconResolver == null || action == null) return;

        UpdateIcon(action);
        UpdateLabel(action);
    }
}
