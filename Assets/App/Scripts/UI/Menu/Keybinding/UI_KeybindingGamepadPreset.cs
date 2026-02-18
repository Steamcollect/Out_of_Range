using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "KeybindingGamepadPreset", menuName = "OutOfRange/UI/Keybinding Gamepad Preset")]
public class UI_KeybindingGamepadPreset : ScriptableObject
{
    [Header("Preset Info")]
    [SerializeField] private string m_PresetName = "Default";
    [SerializeField] private Sprite m_PresetIcon;
    
    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset m_InputActionAsset;
    [SerializeField] private string m_ActionMapName = "Player";
    [SerializeField] private List<ActionBinding> m_ActionBindings = new List<ActionBinding>();
    
    public string PresetName => m_PresetName;
    public Sprite PresetIcon => m_PresetIcon;
    public InputActionAsset InputActionAsset => m_InputActionAsset;
    public string ActionMapName => m_ActionMapName;
    public List<ActionBinding> ActionBindings => m_ActionBindings;
    
    [Serializable]
    public class ActionBinding
    {
        [Tooltip("Name of the action in the InputActionMap")]
        public string ActionName;
        
        [Tooltip("Display name shown in the UI")]
        public string DisplayName;
        
        [Tooltip("Category for grouping actions in the UI")]
        public string Category = "General";
        
        [Tooltip("The gamepad binding path (e.g., <Gamepad>/buttonSouth)")]
        public string BindingPath;
        
        [Tooltip("Icon to display for this binding")]
        public Sprite BindingIcon;
    }
    
    /// <summary>
    /// Applies this preset's bindings to the InputActionAsset
    /// </summary>
    public void ApplyPreset()
    {
        if (m_InputActionAsset == null) return;
        
        var actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return;
        
        foreach (var binding in m_ActionBindings)
        {
            var action = actionMap.FindAction(binding.ActionName);
            if (action == null) continue;
            
            // Find and update gamepad binding
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].groups.Contains("Gamepad"))
                {
                    action.ApplyBindingOverride(i, binding.BindingPath);
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Gets actions grouped by category
    /// </summary>
    public Dictionary<string, List<ActionBinding>> GetActionsByCategory()
    {
        var categories = new Dictionary<string, List<ActionBinding>>();
        
        foreach (var binding in m_ActionBindings)
        {
            if (!categories.ContainsKey(binding.Category))
            {
                categories[binding.Category] = new List<ActionBinding>();
            }
            categories[binding.Category].Add(binding);
        }
        
        return categories;
    }
}