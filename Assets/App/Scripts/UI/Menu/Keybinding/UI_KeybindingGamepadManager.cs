using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager for gamepad keybinding presets.
/// Contains a list of UI_KeybindingGamepadItem, each representing a complete preset.
/// Navigation buttons switch between presets and apply them automatically.
/// </summary>
public class UI_KeybindingGamepadManager : MonoBehaviour
{
    [Header("Preset Items")]
    [SerializeField] private List<UI_KeybindingGamepadItem> m_PresetItems = new();
    [SerializeField] private int m_CurrentPresetIndex = 0;
    
    [Header("Preset Navigation")]
    [SerializeField] private Button m_PreviousPresetButton;
    [SerializeField] private Button m_NextPresetButton;
    [SerializeField] private TextMeshProUGUI m_PresetNameText;
    [SerializeField] private Image m_PresetIconImage;

    public UI_KeybindingGamepadItem CurrentItem => 
        m_PresetItems.Count > 0 && m_CurrentPresetIndex >= 0 && m_CurrentPresetIndex < m_PresetItems.Count 
            ? m_PresetItems[m_CurrentPresetIndex] 
            : null;
    
    private void OnEnable()
    {
        ApplyCurrentPreset();
    }
    
    /// <summary>
    /// Switches to the previous preset in the list
    /// </summary>
    public void PreviousPreset()
    {
        if (m_PresetItems.Count == 0) return;
        
        m_CurrentPresetIndex--;
        if (m_CurrentPresetIndex < 0)
        {
            m_CurrentPresetIndex = m_PresetItems.Count - 1;
        }
        
        ApplyCurrentPreset();
    }
    
    /// <summary>
    /// Switches to the next preset in the list
    /// </summary>
    public void NextPreset()
    {
        if (m_PresetItems.Count == 0) return;
        
        m_CurrentPresetIndex = (m_CurrentPresetIndex + 1) % m_PresetItems.Count;
        
        ApplyCurrentPreset();
    }
    
    /// <summary>
    /// Sets a specific preset by index
    /// </summary>
    public void SetPreset(int index)
    {
        if (index < 0 || index >= m_PresetItems.Count) return;
        
        m_CurrentPresetIndex = index;
        ApplyCurrentPreset();
    }
    
    /// <summary>
    /// Applies the current preset - shows the item UI and applies its keybindings
    /// </summary>
    public void ApplyCurrentPreset()
    {
        // Hide all preset items
        foreach (var item in m_PresetItems)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
            }
        }
        
        // Show and apply current preset
        if (CurrentItem != null)
        {
            CurrentItem.gameObject.SetActive(true);
            CurrentItem.ApplyPreset();
            UpdatePresetDisplay();
        }
        
        UpdateNavigationButtons();
    }
    
    /// <summary>
    /// Updates the preset name and icon display
    /// </summary>
    private void UpdatePresetDisplay()
    {
        if (CurrentItem == null || CurrentItem.Preset == null) return;
        
        if (m_PresetNameText != null)
        {
            m_PresetNameText.text = CurrentItem.Preset.PresetName;
        }
        
        if (m_PresetIconImage != null && CurrentItem.Preset.PresetIcon != null)
        {
            m_PresetIconImage.sprite = CurrentItem.Preset.PresetIcon;
            m_PresetIconImage.enabled = true;
        }
        else if (m_PresetIconImage != null)
        {
            m_PresetIconImage.enabled = false;
        }
    }
    
    /// <summary>
    /// Updates the interactable state of navigation buttons
    /// </summary>
    private void UpdateNavigationButtons()
    {
        bool canNavigate = m_PresetItems.Count > 1;
        
        if (m_PreviousPresetButton != null)
        {
            m_PreviousPresetButton.interactable = canNavigate;
        }
        
        if (m_NextPresetButton != null)
        {
            m_NextPresetButton.interactable = canNavigate;
        }
    }
    
    /// <summary>
    /// Gets the list of all preset items
    /// </summary>
    public List<UI_KeybindingGamepadItem> GetPresetItems()
    {
        return m_PresetItems;
    }
    
    /// <summary>
    /// Gets the current preset index
    /// </summary>
    public int GetCurrentPresetIndex()
    {
        return m_CurrentPresetIndex;
    }
}