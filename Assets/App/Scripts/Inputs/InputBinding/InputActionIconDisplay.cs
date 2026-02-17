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
    [SerializeField] private bool m_AutoUpdateOnDeviceChange = true;
    [SerializeField] private bool m_HideImageIfNoIcon = true;
    [SerializeField] private bool m_ShowTextOnlyOnMouse = true;
    [SerializeField] private bool m_ShowTextIfIcon;
    [SerializeField] private InputActionReference m_ActionReference;

    [Title("References")]
    [SerializeField] 
    private Image m_IconImage;
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;
    [SerializeField] 
    private TMP_Text m_LabelText;
    [Space]
    [SerializeField] 
    private SSO_InputBindingIconResolver m_IconResolver;
    [SerializeField] 
    private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    
    private Sprite m_CurrentIcon;
    
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

        InputAction action = m_ActionReference.action;
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
        m_CurrentIcon = m_IconResolver.GetIconForAction(action);
        if (m_IconImage)
        {
            m_IconImage.sprite = m_CurrentIcon;
            if (m_HideImageIfNoIcon)
            {
                m_IconImage.enabled = m_CurrentIcon;
            }
        }

        if (m_SpriteRenderer)
        {
            m_SpriteRenderer.sprite = m_CurrentIcon;
            if (m_HideImageIfNoIcon)
            {
                m_SpriteRenderer.enabled = m_CurrentIcon;
            }
        }

        
    }

    private void UpdateLabel(InputAction action)
    {
        if (!m_LabelText) return;

        m_LabelText.text = m_IconResolver.GetDisplayNameForAction(action);

        bool hasIcon = m_CurrentIcon != null;
        bool hideTextBecauseIcon = hasIcon && !m_ShowTextIfIcon;
        
        // Option pour afficher le texte uniquement sur clavier/souris
        bool hideTextBecauseNotMouse = m_ShowTextOnlyOnMouse && m_CurrentInputDeviceType.Get() != InputDeviceType.KeyboardMouse;

        m_LabelText.enabled = !hideTextBecauseIcon && !hideTextBecauseNotMouse;
    }
}
