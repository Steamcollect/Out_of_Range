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
    private const string k_LogPrefix = "[InputBindingIconResolver]";
    
    // Control Schemes
    private const string k_ControlSchemeKeyboardMouse = "Keyboard&Mouse";
    private const string k_ControlSchemeKeyboard = "Keyboard";
    private const string k_ControlSchemeMouse = "Mouse";
    private const string k_ControlSchemeGamepad = "Gamepad";
    
    // Regex patterns - supporte les formats: <Device>/control et /Device/control
    private const string k_BindingPathPattern = @"^<?([^>/]+)>?/(.+)$";
    private const string k_CamelCaseSplitPattern = @"([a-z])([A-Z])";
    private const string k_CamelCaseReplacement = "$1 $2";

    [Header("Settings")]
    [SerializeField] 
    private SSO_InputBindingIconSet m_IconSet;
    
    [Header("References")]
    [SerializeField] 
    private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    private static readonly Regex s_BindingPathRegex = new(k_BindingPathPattern, RegexOptions.Compiled);

    public Sprite GetIconForAction(InputAction action)
    {
        if (action == null)
        {
            Debug.LogWarning($"{k_LogPrefix} Action is null");
            return m_IconSet?.DefaultIcon;
        }

        string bindingPath = GetRelevantBindingPath(action, m_CurrentInputDeviceType.Value);
            
        if (string.IsNullOrEmpty(bindingPath))
        {
            return m_IconSet?.DefaultIcon;
        }

        if (m_IconSet.TryGetIconEntry(bindingPath, m_CurrentInputDeviceType.Value, out InputBindingIconEntry entry))
        {
            return entry.Icon != null ? entry.Icon : m_IconSet.DefaultIcon;
        }

        if (m_IconSet.TryGetIconEntryByPath(bindingPath, out InputBindingIconEntry fallbackEntry))
        {
            return fallbackEntry.Icon != null ? fallbackEntry.Icon : m_IconSet.DefaultIcon;
        }

        return m_IconSet?.DefaultIcon;
    }

    
    public string GetDisplayNameForAction(InputAction action)
    {
        if (action == null)
        {
            Debug.LogWarning($"{k_LogPrefix} Action is null");
            return string.Empty;
        }

        string bindingPath = GetRelevantBindingPath(action, m_CurrentInputDeviceType.Value);
            
        return string.IsNullOrEmpty(bindingPath) ? action.name : GetDisplayNameFromPath(bindingPath);
    }

    public Sprite GetIconForBinding(string bindingPath, InputDeviceType deviceType)
    {
        if (string.IsNullOrEmpty(bindingPath))
            return m_IconSet?.DefaultIcon;

        // Normaliser le path pour essayer avec le device générique
        string normalizedPath = NormalizeBindingPath(bindingPath, deviceType);

        // Essayer avec le path normalisé d'abord
        if (m_IconSet.TryGetIconEntry(normalizedPath, deviceType, out InputBindingIconEntry entry))
            return entry.Icon != null ? entry.Icon : m_IconSet.DefaultIcon;

        // Essayer avec le path original
        if (m_IconSet.TryGetIconEntry(bindingPath, deviceType, out InputBindingIconEntry originalEntry))
            return originalEntry.Icon != null ? originalEntry.Icon : m_IconSet.DefaultIcon;

        // Fallback par path
        if (m_IconSet.TryGetIconEntryByPath(normalizedPath, out InputBindingIconEntry fallbackEntry))
            return fallbackEntry.Icon != null ? fallbackEntry.Icon : m_IconSet.DefaultIcon;

        if (m_IconSet.TryGetIconEntryByPath(bindingPath, out InputBindingIconEntry fallbackEntry2))
            return fallbackEntry2.Icon != null ? fallbackEntry2.Icon : m_IconSet.DefaultIcon;

        return m_IconSet?.DefaultIcon;
    }

    public string GetDisplayNameForBinding(string bindingPath, InputDeviceType deviceType)
    {
        if (string.IsNullOrEmpty(bindingPath))
            return string.Empty;

        // Normaliser le path pour essayer avec le device générique
        string normalizedPath = NormalizeBindingPath(bindingPath, deviceType);

        return GetDisplayNameFromPath(normalizedPath);
    }

    /// <summary>
    /// Normalise un binding path vers le format générique du device.
    /// Ex: "/XInputControllerWindows/rightShoulder" devient "&lt;Gamepad&gt;/rightShoulder"
    /// </summary>
    private string NormalizeBindingPath(string bindingPath, InputDeviceType deviceType)
    {
        if (string.IsNullOrEmpty(bindingPath))
            return bindingPath;

        // Normaliser le path pour l'analyse
        string normalizedPath = bindingPath.TrimStart('/');
        
        // Extraire le nom du device et le control
        int slashIndex = normalizedPath.IndexOf('/');
        if (slashIndex <= 0) return bindingPath;
        
        string devicePart = normalizedPath.Substring(0, slashIndex).Trim('<', '>').ToLowerInvariant();
        string controlPart = normalizedPath.Substring(slashIndex + 1);

        // Vérifier si c'est un device spécifique qui hérite d'un device générique
        if (deviceType == InputDeviceType.Gamepad)
        {
            // Si c'est un type de gamepad spécifique, normaliser vers <Gamepad>
            if (devicePart.Contains("xinput") || 
                devicePart.Contains("dualshock") || 
                devicePart.Contains("dualsense") ||
                devicePart.Contains("switch") ||
                devicePart.Contains("gamepad"))
            {
                return $"<Gamepad>/{controlPart}";
            }
        }
        else if (deviceType == InputDeviceType.KeyboardMouse)
        {
            if (devicePart.Contains("keyboard"))
                return $"<Keyboard>/{controlPart}";
            if (devicePart.Contains("mouse"))
                return $"<Mouse>/{controlPart}";
        }

        // Retourner le format standard avec <>
        return $"<{normalizedPath.Substring(0, slashIndex).Trim('<', '>')}>/{controlPart}";
    }


    private string GetRelevantBindingPath(InputAction action, InputDeviceType deviceType)
    {
        if (action == null) return null;

        string compositeName = GetCompositeNameForDeviceType(action, deviceType);
        if (!string.IsNullOrEmpty(compositeName))
        {
            return compositeName;
        }

        string simplePath = GetSimpleBindingPathForDeviceType(action, deviceType);
        
        return simplePath;
    }

    private string GetCompositeNameForDeviceType(InputAction action, InputDeviceType deviceType)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];
            
            if (!binding.isComposite) continue;
            
            if (IsCompositeForDeviceType(action, i, deviceType))
            {
                return !string.IsNullOrEmpty(binding.name) ? binding.name : binding.path;
            }
        }
        
        return null;
    }

    private string GetSimpleBindingPathForDeviceType(InputAction action, InputDeviceType deviceType)
    {
        foreach (InputBinding binding in action.bindings)
        {
            if (binding.isComposite || binding.isPartOfComposite) continue;

            if (IsBindingForDeviceType(binding, deviceType))
            {
                return binding.effectivePath;
            }
        }

        return null;
    }

    private bool IsCompositeForDeviceType(InputAction action, int compositeIndex, InputDeviceType deviceType)
    {
        for (int j = compositeIndex + 1; j < action.bindings.Count; j++)
        {
            InputBinding partBinding = action.bindings[j];
            
            if (partBinding.isComposite || !partBinding.isPartOfComposite)
                break;
            
            if (IsBindingForDeviceType(partBinding, deviceType))
            {
                return true;
            }
        }
        
        return false;
    }

    private bool IsBindingForDeviceType(InputBinding binding, InputDeviceType deviceType)
    {
        string targetControlScheme = deviceType switch
        {
            InputDeviceType.KeyboardMouse => k_ControlSchemeKeyboardMouse,
            InputDeviceType.Gamepad => k_ControlSchemeGamepad,
            _ => null
        };
        
        if (!string.IsNullOrEmpty(binding.groups))
        {
            bool matchesScheme = binding.groups.Contains(targetControlScheme, StringComparison.OrdinalIgnoreCase);
            
            if (!matchesScheme && deviceType == InputDeviceType.KeyboardMouse)
            {
                matchesScheme = binding.groups.Contains(k_ControlSchemeKeyboard, StringComparison.OrdinalIgnoreCase) ||
                               binding.groups.Contains(k_ControlSchemeMouse, StringComparison.OrdinalIgnoreCase);
            }
            
            return matchesScheme;
        }
        
        return IsBindingPathForDeviceType(binding.effectivePath, deviceType);
    }

    private bool IsBindingPathForDeviceType(string bindingPath, InputDeviceType deviceType)
    {
        if (string.IsNullOrEmpty(bindingPath)) return false;

        
        // Normaliser le path pour l'analyse
        string normalizedPath = bindingPath.TrimStart('/');
        
        // Extraire le nom du device et le control
        int slashIndex = normalizedPath.IndexOf('/');
        if (slashIndex <= 0) return false;
        
        string devicePart = normalizedPath.Substring(0, slashIndex).Trim('<', '>').ToLowerInvariant();
        string controlPart = normalizedPath.Substring(slashIndex + 1);

        // Méthode 1: Essayer de trouver le control avec différents formats de path
        string[] pathsToTry = new[]
        {
            bindingPath,
            $"<{devicePart}>/{controlPart}",
            $"<Gamepad>/{controlPart}",
            $"<Keyboard>/{controlPart}",
            $"<Mouse>/{controlPart}"
        };

        foreach (string pathToTry in pathsToTry)
        {
            InputControl control = InputSystem.FindControl(pathToTry);
            if (control != null)
            {
                InputDevice device = control.device;
                bool matches = deviceType switch
                {
                    InputDeviceType.KeyboardMouse => device is Keyboard or Mouse,
                    InputDeviceType.Gamepad => device is Gamepad,
                    _ => false
                };
                if (matches) return true;
            }
        }

        // Méthode 2: Fallback par analyse du nom du device dans le path
        return deviceType switch
        {
            InputDeviceType.KeyboardMouse => devicePart.Contains("keyboard") || devicePart.Contains("mouse"),
            InputDeviceType.Gamepad => devicePart.Contains("gamepad") || 
                                       devicePart.Contains("xinput") || 
                                       devicePart.Contains("dualshock") || 
                                       devicePart.Contains("dualsense") ||
                                       devicePart.Contains("switch"),
            _ => false
        };
    }


    private string GetDisplayNameFromPath(string bindingPath)
    {
        if (string.IsNullOrEmpty(bindingPath))
        {
            return string.Empty;
        }

        // Normaliser le path
        string normalizedPath = bindingPath.TrimStart('/');
        
        Match match = s_BindingPathRegex.Match(normalizedPath);
        
        string controlId;
        if (match.Success)
        {
            controlId = match.Groups[2].Value;
        }
        else
        {
            // Fallback: extraire la partie après le premier /
            int slashIndex = normalizedPath.IndexOf('/');
            if (slashIndex >= 0 && slashIndex < normalizedPath.Length - 1)
            {
                controlId = normalizedPath.Substring(slashIndex + 1);
            }
            else
            {
                return bindingPath;
            }
        }

        if (m_IconSet != null && m_IconSet.TryGetIconEntryByPath(bindingPath, out var entry) 
                              && !string.IsNullOrEmpty(entry.CustomDisplayName))
        {
            return entry.CustomDisplayName;
        }

        if (m_IconSet != null)
        {
            var overrideName = m_IconSet.GetDisplayNameOverride(controlId);
            if (!string.IsNullOrEmpty(overrideName))
            {
                return overrideName;
            }
        }

        return FormatControlIdAsDisplayName(controlId);
    }

    private string FormatControlIdAsDisplayName(string controlId)
    {
        if (string.IsNullOrEmpty(controlId))
        {
            return string.Empty;
        }

        if (controlId.Length == 1)
        {
            return controlId.ToUpperInvariant();
        }

        string result = Regex.Replace(controlId, k_CamelCaseSplitPattern, k_CamelCaseReplacement);
            
        if (result.Length > 0)
        {
            result = char.ToUpperInvariant(result[0]) + result.Substring(1);
        }

        return result;
    }
}