using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SSO_InputBindingIconSet", menuName = "SSO/Input/InputBindingIconSet")]
public class SSO_InputBindingIconSet : ScriptableObject
{
    
    // Button labels
    private const string k_ButtonAddKeyboard = "Ajouter Keyboard Entry";
    private const string k_ButtonAddMouse = "Ajouter Mouse Entry";
    private const string k_ButtonAddGamepad = "Ajouter Gamepad Entry";
    
    // Default binding paths
    private const string k_DefaultKeyboardPath = "<Keyboard>/";
    private const string k_DefaultMousePath = "<Mouse>/";
    private const string k_DefaultGamepadPath = "<Gamepad>/";

    [Title("Settings")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    [SerializeField] 
    private List<InputBindingIconEntry> m_IconEntries = new();
    [Space]
    [DictionaryDrawerSettings(KeyLabel = "Control ID", ValueLabel = "Label affiché")]
    [SerializeField]
    private SerializedDictionary<string, string> m_DisplayNameOverrides = new()
    {
        // Keyboard
        { "space", "Space" },
        { "escape", "Esc" },
        { "enter", "Enter" },
        { "tab", "Tab" },
        { "backspace", "Backspace" },
        { "leftShift", "Left Shift" },
        { "rightShift", "Right Shift" },
        { "leftCtrl", "Left Ctrl" },
        { "rightCtrl", "Right Ctrl" },
        { "leftAlt", "Left Alt" },
        { "rightAlt", "Right Alt" },
        { "upArrow", "↑" },
        { "downArrow", "↓" },
        { "leftArrow", "←" },
        { "rightArrow", "→" },
            
        // Mouse
        { "leftButton", "Left Click" },
        { "rightButton", "Right Click" },
        { "middleButton", "Middle Click" },
        { "scroll", "Scroll" },
        { "delta", "Mouse Move" },
            
        // Gamepad - Sticks
        { "leftStick", "Left Stick" },
        { "rightStick", "Right Stick" },
        { "leftStickPress", "L3" },
        { "rightStickPress", "R3" },
            
        // Gamepad - Triggers & Bumpers
        { "leftTrigger", "LT" },
        { "rightTrigger", "RT" },
        { "leftShoulder", "LB" },
        { "rightShoulder", "RB" },
            
        // Gamepad - Face Buttons (Xbox style - peut être ajusté selon plateforme)
        { "buttonSouth", "A" },
        { "buttonNorth", "Y" },
        { "buttonEast", "B" },
        { "buttonWest", "X" },
            
        // Gamepad - D-Pad
        { "dpad", "D-Pad" },
        { "dpad/up", "D-Pad Up" },
        { "dpad/down", "D-Pad Down" },
        { "dpad/left", "D-Pad Left" },
        { "dpad/right", "D-Pad Right" },
            
        // Gamepad - Special
        { "start", "Start" },
        { "select", "Select" }
    };
    [Space]
    [SerializeField] 
    private Sprite m_DefaultIcon;
    

    public IReadOnlyList<InputBindingIconEntry> IconEntries => m_IconEntries;

    public Sprite DefaultIcon => m_DefaultIcon;

    public IReadOnlyDictionary<string, string> DisplayNameOverrides => m_DisplayNameOverrides;
    
    public bool TryGetIconEntry(string bindingPath, InputDeviceType deviceType, out InputBindingIconEntry entry)
    {
        // Recherche exacte (path + device type)
        foreach (var iconEntry in m_IconEntries)
        {
            if (string.Equals(iconEntry.BindingPath, bindingPath, System.StringComparison.OrdinalIgnoreCase) 
                && iconEntry.DeviceType == deviceType)
            {
                entry = iconEntry;
                return true;
            }
        }

        entry = default;
        return false;
    }

    public bool TryGetIconEntryByPath(string bindingPath, out InputBindingIconEntry entry)
    {
        foreach (var iconEntry in m_IconEntries)
        {
            if (string.Equals(iconEntry.BindingPath, bindingPath, System.StringComparison.OrdinalIgnoreCase))
            {
                entry = iconEntry;
                return true;
            }
        }

        entry = default;
        return false;
    }


    public string GetDisplayNameOverride(string controlId)
    {
        return m_DisplayNameOverrides.GetValueOrDefault(controlId);
    }

#if UNITY_EDITOR
    [Button(k_ButtonAddKeyboard), GUIColor(0.4f, 0.8f, 0.4f)]
    private void AddKeyboardEntry()
    {
        m_IconEntries.Add(new InputBindingIconEntry
        {
            BindingPath = k_DefaultKeyboardPath,
            DeviceType = InputDeviceType.KeyboardMouse,
            Icon = null,
            CustomDisplayName = string.Empty
        });
    }

    [Button(k_ButtonAddMouse), GUIColor(0.4f, 0.6f, 0.8f)]
    private void AddMouseEntry()
    {
        m_IconEntries.Add(new InputBindingIconEntry
        {
            BindingPath = k_DefaultMousePath,
            DeviceType = InputDeviceType.KeyboardMouse,
            Icon = null,
            CustomDisplayName = string.Empty
        });
    }

    [Button(k_ButtonAddGamepad), GUIColor(0.8f, 0.6f, 0.4f)]
    private void AddGamepadEntry()
    {
        m_IconEntries.Add(new InputBindingIconEntry
        {
            BindingPath = k_DefaultGamepadPath,
            DeviceType = InputDeviceType.Gamepad,
            Icon = null,
            CustomDisplayName = string.Empty
        });
    }
#endif
}