using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// ScriptableObject contenant la configuration des icônes pour les binding paths.
/// Permet de mapper des binding paths Unity Input System vers des icônes visuelles.
/// </summary>
[CreateAssetMenu(fileName = "SSO_InputBindingIconSet", menuName = "SSO/Input/InputBindingIconSet")]
public class SSO_InputBindingIconSet : ScriptableObject
{
    [Title("Configuration des icônes")]
    [InfoBox("Ajoutez les binding paths avec leurs icônes correspondantes pour chaque type de périphérique.")]
    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    [SerializeField] 
    private List<InputBindingIconEntry> m_IconEntries = new();

    [Title("Icône par défaut")]
    [Tooltip("Icône à utiliser si aucun binding path ne correspond")]
    [SerializeField] 
    private Sprite m_DefaultIcon;

    [Title("Table de mapping des labels")]
    [InfoBox("Personnalisez les labels humains pour des contrôles spécifiques.")]
    [DictionaryDrawerSettings(KeyLabel = "Control ID", ValueLabel = "Label affiché")]
    [SerializeField]
    private Dictionary<string, string> m_DisplayNameOverrides = new()
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

    /// <summary>
    /// Obtient toutes les entrées d'icônes configurées.
    /// </summary>
    public IReadOnlyList<InputBindingIconEntry> IconEntries => m_IconEntries;

    /// <summary>
    /// Obtient l'icône par défaut.
    /// </summary>
    public Sprite DefaultIcon => m_DefaultIcon;

    /// <summary>
    /// Obtient la table de mapping des labels personnalisés.
    /// </summary>
    public IReadOnlyDictionary<string, string> DisplayNameOverrides => m_DisplayNameOverrides;

    /// <summary>
    /// Recherche une entrée d'icône pour un binding path et un type de périphérique donnés.
    /// </summary>
    /// <param name="bindingPath">Le binding path à rechercher</param>
    /// <param name="deviceType">Le type de périphérique souhaité</param>
    /// <param name="entry">L'entrée trouvée si elle existe</param>
    /// <returns>True si une entrée correspondante a été trouvée</returns>
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

    /// <summary>
    /// Recherche une entrée d'icône pour un binding path (sans filtrage par device type).
    /// </summary>
    /// <param name="bindingPath">Le binding path à rechercher</param>
    /// <param name="entry">L'entrée trouvée si elle existe</param>
    /// <returns>True si une entrée correspondante a été trouvée</returns>
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

    /// <summary>
    /// Obtient le label personnalisé pour un contrôle, ou null s'il n'existe pas.
    /// </summary>
    /// <param name="controlId">L'identifiant du contrôle (partie après '/' dans le path)</param>
    /// <returns>Le label personnalisé ou null</returns>
    public string GetDisplayNameOverride(string controlId)
    {
        return m_DisplayNameOverrides.TryGetValue(controlId, out var displayName) ? displayName : null;
    }

#if UNITY_EDITOR
    [Button("Ajouter Keyboard Entry"), GUIColor(0.4f, 0.8f, 0.4f)]
    private void AddKeyboardEntry()
    {
        m_IconEntries.Add(new InputBindingIconEntry
        {
            BindingPath = "<Keyboard>/",
            DeviceType = InputDeviceType.KeyboardMouse,
            Icon = null,
            CustomDisplayName = ""
        });
    }

    [Button("Ajouter Mouse Entry"), GUIColor(0.4f, 0.6f, 0.8f)]
    private void AddMouseEntry()
    {
        m_IconEntries.Add(new InputBindingIconEntry
        {
            BindingPath = "<Mouse>/",
            DeviceType = InputDeviceType.KeyboardMouse,
            Icon = null,
            CustomDisplayName = ""
        });
    }

    [Button("Ajouter Gamepad Entry"), GUIColor(0.8f, 0.6f, 0.4f)]
    private void AddGamepadEntry()
    {
        m_IconEntries.Add(new InputBindingIconEntry
        {
            BindingPath = "<Gamepad>/",
            DeviceType = InputDeviceType.Gamepad,
            Icon = null,
            CustomDisplayName = ""
        });
    }
#endif
}