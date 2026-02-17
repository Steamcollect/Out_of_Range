using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Service de résolution d'icônes et de labels pour les InputActions.
/// Fournit l'API publique pour obtenir des icônes et des noms lisibles basés sur le périphérique actif.
/// </summary>
[CreateAssetMenu(fileName = "SSO_InputBindingIconResolver", menuName = "SSO/Input/InputBindingIconResolver")]
public sealed class SSO_InputBindingIconResolver : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] 
    private SSO_InputBindingIconSet m_IconSet;
    [Header("References")]
    [SerializeField] 
    private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    // Regex pour parser un binding path: <Device>/control
    private static readonly Regex s_BindingPathRegex = new(@"^<([^>]+)>/(.+)$", RegexOptions.Compiled);

    /// <summary>
    /// Obtient l'icône appropriée pour une InputAction en fonction du périphérique actif.
    /// </summary>
    /// <param name="action">L'InputAction pour laquelle obtenir l'icône</param>
    /// <returns>Le Sprite de l'icône, ou l'icône par défaut si aucune correspondance</returns>
    public Sprite GetIconForAction(InputAction action)
    {
        if (action == null)
        {
            Debug.LogWarning("[InputBindingIconResolver] Action is null");
            return m_IconSet?.DefaultIcon;
        }

        string bindingPath = GetRelevantBindingPath(action, m_CurrentInputDeviceType.Value);
            
        if (string.IsNullOrEmpty(bindingPath))
        {
            return m_IconSet?.DefaultIcon;
        }

        // Essayer de trouver une icône exacte pour ce path et device type
        if (m_IconSet.TryGetIconEntry(bindingPath, m_CurrentInputDeviceType.Value, out InputBindingIconEntry entry))
        {
            return entry.Icon != null ? entry.Icon : m_IconSet.DefaultIcon;
        }

        // Fallback: chercher par path uniquement
        if (m_IconSet.TryGetIconEntryByPath(bindingPath, out InputBindingIconEntry fallbackEntry))
        {
            return fallbackEntry.Icon != null ? fallbackEntry.Icon : m_IconSet.DefaultIcon;
        }

        return m_IconSet?.DefaultIcon;
    }

    /// <summary>
    /// Obtient le nom d'affichage lisible pour une InputAction en fonction du périphérique actif.
    /// </summary>
    /// <param name="action">L'InputAction pour laquelle obtenir le nom</param>
    /// <returns>Un label lisible (ex: "A", "Left Click", "Space")</returns>
    public string GetDisplayNameForAction(InputAction action)
    {
        if (action == null)
        {
            Debug.LogWarning("[InputBindingIconResolver] Action is null");
            return string.Empty;
        }

        string bindingPath = GetRelevantBindingPath(action, m_CurrentInputDeviceType.Value);
            
        if (string.IsNullOrEmpty(bindingPath))
        {
            return action.name;
        }

        return GetDisplayNameFromPath(bindingPath);
    }

    /// <summary>
    /// Obtient à la fois l'icône et le nom d'affichage pour une InputAction.
    /// </summary>
    /// <param name="action">L'InputAction à résoudre</param>
    /// <returns>Un tuple contenant l'icône et le nom d'affichage</returns>
    public (Sprite icon, string displayName) GetIconAndDisplayNameForAction(InputAction action)
    {
        return (GetIconForAction(action), GetDisplayNameForAction(action));
    }

    /// <summary>
    /// Obtient le binding path pertinent pour une action selon le type de périphérique actif.
    /// </summary>
    /// <param name="action">L'InputAction</param>
    /// <param name="deviceType">Le type de périphérique souhaité</param>
    /// <returns>Le binding path correspondant, ou null si non trouvé</returns>
    public string GetRelevantBindingPath(InputAction action, InputDeviceType deviceType)
    {
        if (action == null) return null;

        string targetControlScheme = deviceType switch
        {
            InputDeviceType.KeyboardMouse => "Keyboard&Mouse",
            InputDeviceType.Gamepad => "Gamepad",
            _ => null
        };

        // Parcourir les bindings de l'action
        foreach (InputBinding binding in action.bindings)
        {
            // Ignorer les bindings composites (on veut les parties individuelles)
            if (binding.isComposite) continue;
                
            // Ignorer les parties de composite pour le path principal
            if (binding.isPartOfComposite) continue;

            // Vérifier si le binding correspond au control scheme souhaité
            if (!string.IsNullOrEmpty(binding.groups))
            {
                bool matchesScheme = binding.groups.Contains(targetControlScheme, StringComparison.OrdinalIgnoreCase);
                    
                // Pour KeyboardMouse, accepter aussi Keyboard ou Mouse seul
                if (!matchesScheme && deviceType == InputDeviceType.KeyboardMouse)
                {
                    matchesScheme = binding.groups.Contains("Keyboard", StringComparison.OrdinalIgnoreCase) ||
                                    binding.groups.Contains("Mouse", StringComparison.OrdinalIgnoreCase);
                }

                if (matchesScheme)
                {
                    return binding.effectivePath;
                }
            }
            else
            {
                // Si pas de groupe défini, vérifier par le type de device dans le path
                if (IsBindingPathForDeviceType(binding.effectivePath, deviceType))
                {
                    return binding.effectivePath;
                }
            }
        }

        // Fallback: retourner le premier binding non-composite trouvé
        foreach (var binding in action.bindings)
        {
            if (binding is { isComposite: false, isPartOfComposite: false } && !string.IsNullOrEmpty(binding.effectivePath))
            {
                return binding.effectivePath;
            }
        }

        return null;
    }

    /// <summary>
    /// Vérifie si un binding path correspond à un type de périphérique.
    /// </summary>
    private bool IsBindingPathForDeviceType(string bindingPath, InputDeviceType deviceType)
    {
        if (string.IsNullOrEmpty(bindingPath)) return false;

        var pathLower = bindingPath.ToLowerInvariant();

        return deviceType switch
        {
            InputDeviceType.KeyboardMouse => pathLower.Contains("<keyboard>") || pathLower.Contains("<mouse>"),
            InputDeviceType.Gamepad => pathLower.Contains("<gamepad>") || 
                                       pathLower.Contains("<xinputcontroller>") ||
                                       pathLower.Contains("<dualshockgamepad>") ||
                                       pathLower.Contains("<switchprocontroller>"),
            _ => false
        };
    }

    /// <summary>
    /// Extrait un nom d'affichage lisible à partir d'un binding path.
    /// </summary>
    /// <param name="bindingPath">Le binding path (ex: "&lt;Keyboard&gt;/z")</param>
    /// <returns>Un label lisible (ex: "Z")</returns>
    public string GetDisplayNameFromPath(string bindingPath)
    {
        if (string.IsNullOrEmpty(bindingPath))
        {
            return string.Empty;
        }

        // Parser le path avec la regex
        var match = s_BindingPathRegex.Match(bindingPath);
        if (!match.Success)
        {
            return bindingPath;
        }

        string controlId = match.Groups[2].Value;

        // Vérifier d'abord si on a une entrée d'icône avec un CustomDisplayName
        if (m_IconSet != null && m_IconSet.TryGetIconEntryByPath(bindingPath, out var entry) 
                              && !string.IsNullOrEmpty(entry.CustomDisplayName))
        {
            return entry.CustomDisplayName;
        }

        // Ensuite vérifier la table de mapping des labels
        if (m_IconSet != null)
        {
            var overrideName = m_IconSet.GetDisplayNameOverride(controlId);
            if (!string.IsNullOrEmpty(overrideName))
            {
                return overrideName;
            }
        }

        // Fallback: formatter le controlId de manière lisible
        return FormatControlIdAsDisplayName(controlId);
    }

    /// <summary>
    /// Formate un control ID en nom d'affichage lisible.
    /// </summary>
    /// <param name="controlId">L'ID du contrôle (ex: "leftStick", "buttonSouth")</param>
    /// <returns>Un nom formaté (ex: "Left Stick", "Button South")</returns>
    private string FormatControlIdAsDisplayName(string controlId)
    {
        if (string.IsNullOrEmpty(controlId))
        {
            return string.Empty;
        }

        // Si c'est une seule lettre (touche clavier), mettre en majuscule
        if (controlId.Length == 1)
        {
            return controlId.ToUpperInvariant();
        }

        // Ajouter des espaces avant les majuscules (camelCase -> "Camel Case")
        string result = Regex.Replace(controlId, @"([a-z])([A-Z])", "$1 $2");
            
        // Capitaliser la première lettre
        if (result.Length > 0)
        {
            result = char.ToUpperInvariant(result[0]) + result.Substring(1);
        }

        return result;
    }

#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button("Test - Log Current Device Bindings")]
    private void TestLogCurrentBindings()
    {
        Debug.Log($"[InputBindingIconResolver] Current Device Type: {m_CurrentInputDeviceType?.Value}");
        Debug.Log($"[InputBindingIconResolver] Icon Set: {(m_IconSet != null ? m_IconSet.name : "NULL")}");
    }
#endif
}