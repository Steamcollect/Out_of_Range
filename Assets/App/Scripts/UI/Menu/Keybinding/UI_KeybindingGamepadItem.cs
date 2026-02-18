using UnityEngine;

/// <summary>
/// Represents a gamepad preset item with its own UI display.
/// Contains a reference to a UI_KeybindingGamepadPreset which defines the keybindings.
/// When activated, applies the preset bindings to the InputActionAsset.
/// </summary>
public class UI_KeybindingGamepadItem : MonoBehaviour
{
    [Header("Preset Configuration")]
    [SerializeField] private UI_KeybindingGamepadPreset m_Preset;
    
    [Header("UI Display (Optional)")]
    [SerializeField] private GameObject m_DisplayContent;
    
    public UI_KeybindingGamepadPreset Preset => m_Preset;
    
    /// <summary>
    /// Applies the preset's keybindings to the InputActionAsset
    /// Called automatically when this item becomes the active preset
    /// </summary>
    public void ApplyPreset()
    {
        if (m_Preset != null)
        {
            m_Preset.ApplyPreset();
        }
    }
    
    /// <summary>
    /// Shows the display content
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        if (m_DisplayContent != null)
        {
            m_DisplayContent.SetActive(true);
        }
    }
    
    /// <summary>
    /// Hides the display content
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        if (m_DisplayContent != null)
        {
            m_DisplayContent.SetActive(false);
        }
    }
}