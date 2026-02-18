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
    
    // Device Paths
    private const string k_DevicePathKeyboard = "<keyboard>";
    private const string k_DevicePathMouse = "<mouse>";
    private const string k_DevicePathGamepad = "<gamepad>";
    private const string k_DevicePathXinput = "<xinputcontroller>";
    private const string k_DevicePathDualshock = "<dualshockgamepad>";
    private const string k_DevicePathSwitch = "<switchprocontroller>";
    
    // Regex pattern
    private const string k_BindingPathPattern = "^<([^>]+)>/(.+)$";
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

        var pathLower = bindingPath.ToLowerInvariant();

        return deviceType switch
        {
            InputDeviceType.KeyboardMouse => pathLower.Contains(k_DevicePathKeyboard) || 
                                             pathLower.Contains(k_DevicePathMouse),
            InputDeviceType.Gamepad => pathLower.Contains(k_DevicePathGamepad) || 
                                       pathLower.Contains(k_DevicePathXinput) ||
                                       pathLower.Contains(k_DevicePathDualshock) ||
                                       pathLower.Contains(k_DevicePathSwitch),
            _ => false
        };
    }

    private string GetDisplayNameFromPath(string bindingPath)
    {
        if (string.IsNullOrEmpty(bindingPath))
        {
            return string.Empty;
        }

        Match match = s_BindingPathRegex.Match(bindingPath);
        if (!match.Success)
        {
            return bindingPath;
        }

        string controlId = match.Groups[2].Value;

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