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
    [Title("Configuration")]
    [Tooltip("Référence à l'InputAction à afficher")]
    [SerializeField] 
    private InputActionReference m_ActionReference;

    [Tooltip("Le resolver d'icônes à utiliser")]
    [SerializeField] 
    private SSO_InputBindingIconResolver m_IconResolver;

    [Tooltip("Référence au RSO du type de périphérique actuel")]
    [SerializeField] 
    private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    [Title("Affichage")]
    [Tooltip("Image UI pour afficher l'icône (optionnel)")]
    [SerializeField] 
    private Image m_IconImage;

    [Tooltip("TextMeshPro pour afficher le label (optionnel)")]
    [SerializeField] 
    private TMP_Text m_LabelText;

    [Title("Options")]
    [Tooltip("Mettre à jour automatiquement lors du changement de périphérique")]
    [SerializeField] 
    private bool m_AutoUpdateOnDeviceChange = true;

    [Tooltip("Cacher l'image si aucune icône n'est disponible")]
    [SerializeField] 
    private bool m_HideImageIfNoIcon = true;

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
    [Button("Rafraîchir l'affichage")]
    public void UpdateDisplay()
    {
        if (m_IconResolver == null || m_ActionReference == null)
        {
            Debug.LogWarning($"[InputActionIconDisplay] Missing references on {gameObject.name}");
            return;
        }

        var action = m_ActionReference.action;
        if (action == null)
        {
            Debug.LogWarning($"[InputActionIconDisplay] Action is null for {m_ActionReference.name}");
            return;
        }

        // Mettre à jour l'icône
        if (m_IconImage != null)
        {
            var icon = m_IconResolver.GetIconForAction(action);
            m_IconImage.sprite = icon;

            if (m_HideImageIfNoIcon)
            {
                m_IconImage.gameObject.SetActive(icon != null);
            }
        }

        // Mettre à jour le label
        if (m_LabelText != null)
        {
            m_LabelText.text = m_IconResolver.GetDisplayNameForAction(action);
        }
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
        // Note: Pour utiliser directement une InputAction, il faut créer une référence temporaire
        // ou utiliser directement le resolver
        if (m_IconResolver != null && action != null)
        {
            if (m_IconImage != null)
            {
                var icon = m_IconResolver.GetIconForAction(action);
                m_IconImage.sprite = icon;
                if (m_HideImageIfNoIcon)
                {
                    m_IconImage.gameObject.SetActive(icon != null);
                }
            }

            if (m_LabelText != null)
            {
                m_LabelText.text = m_IconResolver.GetDisplayNameForAction(action);
            }
        }
    }
}
