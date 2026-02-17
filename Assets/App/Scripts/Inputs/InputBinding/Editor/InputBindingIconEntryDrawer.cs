#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

/// <summary>
/// Odin Attribute Drawer pour InputBindingPathAttribute.
/// Affiche un champ texte avec un bouton Listen pour capturer les inputs.
/// </summary>
public class InputBindingPathAttributeDrawer : OdinAttributeDrawer<InputBindingPathAttribute, string>
{
    private const string k_ListenButtonText = "Listen";
    private const string k_CancelButtonText = "Cancel";
    private const string k_WaitingText = "Press any key...";
    private const string k_SelectPathText = "Select path...";
    private const float k_ButtonWidth = 50f;
    private const float k_ListenDelaySeconds = 0.15f;

    // Device path alternatives mapping
    private static readonly Dictionary<string, string[]> s_DevicePathAlternatives = new()
    {
        { "XInputControllerWindows", new[] { "<XInputController>", "<Gamepad>" } },
        { "XInputController", new[] { "<XInputController>", "<Gamepad>" } },
        { "DualShockGamepad", new[] { "<DualShockGamepad>", "<Gamepad>" } },
        { "DualShock4GamepadHID", new[] { "<DualShockGamepad>", "<Gamepad>" } },
        { "DualSenseGamepadHID", new[] { "<DualSenseGamepad>", "<Gamepad>" } },
        { "SwitchProControllerHID", new[] { "<SwitchProController>", "<Gamepad>" } },
    };

    private static InspectorProperty s_ListeningProperty;
    private static InspectorProperty s_DeviceTypeProperty;
    private static string s_ListeningPropertyPath; // Identifiant unique basé sur le chemin
    private static bool s_IsListening;
    private static double s_ListenStartTime;
    private static InputDevice s_CapturedDevice;
    private static string s_CapturedControlPath;
    
    // Pour l'affichage du menu de sélection
    private static bool s_ShowingPathSelection;
    private static string[] s_PathOptions;
    private static string s_ControlPart;

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var rect = EditorGUILayout.GetControlRect();

        if (label != null)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        // Calculer les rects
        float buttonWidth = k_ButtonWidth;
        Rect fieldRect = new Rect(rect.x, rect.y, rect.width - buttonWidth - 2, rect.height);
        Rect buttonRect = new Rect(rect.xMax - buttonWidth, rect.y, buttonWidth, rect.height);

        // Utiliser le chemin de la propriété pour identifier de manière unique
        string currentPropertyPath = this.Property.Path;
        bool isThisPropertyListening = s_IsListening && s_ListeningPropertyPath == currentPropertyPath;
        bool isThisPropertyShowingSelection = s_ShowingPathSelection && s_ListeningPropertyPath == currentPropertyPath;

        if (isThisPropertyShowingSelection && s_PathOptions != null)
        {
            // Afficher le dropdown de sélection
            DrawPathSelectionDropdown(fieldRect, buttonRect);
        }
        else if (isThisPropertyListening)
        {
            // Afficher "Press any key..."
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(fieldRect, k_WaitingText);
            EditorGUI.EndDisabledGroup();

            if (GUI.Button(buttonRect, k_CancelButtonText))
            {
                StopListening();
            }

            // Force le repaint pour garder l'UI réactive
            GUIHelper.RequestRepaint();
        }
        else
        {
            // Champ texte normal
            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUI.TextField(fieldRect, this.ValueEntry.SmartValue);
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = newValue;
            }

            // Bouton Listen
            EditorGUI.BeginDisabledGroup(s_IsListening || s_ShowingPathSelection);
            if (GUI.Button(buttonRect, k_ListenButtonText))
            {
                StartListening();
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    private void DrawPathSelectionDropdown(Rect fieldRect, Rect buttonRect)
    {
        // Créer les options pour le popup
        string[] displayOptions = new string[s_PathOptions.Length + 1];
        displayOptions[0] = k_SelectPathText;
        for (int i = 0; i < s_PathOptions.Length; i++)
        {
            displayOptions[i + 1] = $"{s_PathOptions[i]}{s_ControlPart}";
        }

        EditorGUI.BeginChangeCheck();
        int selected = EditorGUI.Popup(fieldRect, 0, displayOptions);
        if (EditorGUI.EndChangeCheck() && selected > 0)
        {
            string selectedPath = displayOptions[selected];
            ApplySelectedPath(selectedPath);
        }

        if (GUI.Button(buttonRect, k_CancelButtonText))
        {
            StopListening();
        }

        GUIHelper.RequestRepaint();
    }

    private void StartListening()
    {
        s_IsListening = true;
        s_ShowingPathSelection = false;
        s_ListeningProperty = this.Property;
        s_ListeningPropertyPath = this.Property.Path;
        s_ListenStartTime = EditorApplication.timeSinceStartup;
        s_CapturedDevice = null;
        s_CapturedControlPath = null;
        s_PathOptions = null;
        s_ControlPart = null;

        // Trouver la propriété DeviceType si spécifiée
        if (!string.IsNullOrEmpty(this.Attribute.DeviceTypeFieldName))
        {
            s_DeviceTypeProperty = this.Property.Parent?.Children[this.Attribute.DeviceTypeFieldName];
        }

        InputSystem.onEvent += OnInputEvent;
        EditorApplication.update += ForceRepaint;
    }

    private static void StopListening()
    {
        s_IsListening = false;
        s_ShowingPathSelection = false;
        s_ListeningProperty = null;
        s_ListeningPropertyPath = null;
        s_DeviceTypeProperty = null;
        s_CapturedDevice = null;
        s_CapturedControlPath = null;
        s_PathOptions = null;
        s_ControlPart = null;

        InputSystem.onEvent -= OnInputEvent;
        EditorApplication.update -= ForceRepaint;
    }

    private static void ForceRepaint()
    {
        if (s_IsListening || s_ShowingPathSelection)
        {
            GUIHelper.RequestRepaint();
        }
    }

    private static void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!s_IsListening || s_ListeningProperty == null) return;

        // Vérifier le délai d'attente
        if (EditorApplication.timeSinceStartup - s_ListenStartTime < k_ListenDelaySeconds) return;

        // Ignorer les events qui ne sont pas des changements d'état
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

        // Pour les gamepads, utiliser un seuil plus bas pour les boutons
        float magnitudeThreshold = 0.2f;

        // Trouver le contrôle qui a été actualisé
        foreach (var control in eventPtr.EnumerateChangedControls(device, magnitudeThreshold))
        {
            // Ignorer certains contrôles non pertinents
            if (IsIgnoredControl(control)) continue;

            // Lire la valeur actuelle du contrôle depuis l'événement
            float value = 0f;
            if (control is UnityEngine.InputSystem.Controls.ButtonControl buttonControl)
            {
                value = buttonControl.ReadValueFromEvent(eventPtr);
                // Ignorer si le bouton n'est pas suffisamment pressé
                if (value < 0.5f) continue;
            }
            else if (control is UnityEngine.InputSystem.Controls.AxisControl axisControl)
            {
                value = Mathf.Abs(axisControl.ReadValueFromEvent(eventPtr));
                // Pour les axes (sticks), exiger une valeur plus élevée
                if (value < 0.5f) continue;
            }

            Debug.Log($"[InputBindingPathDrawer] Captured: {control.path} (value: {value})");

            // Stocker les infos capturées
            s_CapturedDevice = device;
            s_CapturedControlPath = control.path;

            // Désabonner immédiatement pour éviter les doublons
            InputSystem.onEvent -= OnInputEvent;

            // Vérifier si des alternatives existent
            string deviceName = GetDeviceName(control.path);
            if (s_DevicePathAlternatives.TryGetValue(deviceName, out string[] alternatives) && alternatives.Length > 1)
            {
                // Passer en mode sélection de path
                s_IsListening = false;
                s_ShowingPathSelection = true;
                s_PathOptions = alternatives;
                s_ControlPart = GetControlPart(control.path);
                Debug.Log($"[InputBindingPathDrawer] Showing path selection for device: {deviceName}");
            }
            else
            {
                // Appliquer directement
                Debug.Log($"[InputBindingPathDrawer] Applying default path for device: {deviceName}");
                ApplyDefaultPath();
            }
            
            return;
        }
    }

    private static void ApplySelectedPath(string bindingPath)
    {
        if (s_ListeningProperty == null) return;

        // Mettre à jour le binding path
        s_ListeningProperty.ValueEntry.WeakSmartValue = bindingPath;

        // Détecter et mettre à jour le device type automatiquement
        if (s_DeviceTypeProperty != null && s_CapturedDevice != null)
        {
            InputDeviceType detectedType = DetectDeviceType(s_CapturedDevice);
            s_DeviceTypeProperty.ValueEntry.WeakSmartValue = detectedType;
        }

        StopListening();
    }

    private static void ApplyDefaultPath()
    {
        if (s_CapturedControlPath == null) return;

        string bindingPath = FormatBindingPath(s_CapturedControlPath);
        ApplySelectedPath(bindingPath);
    }

    private static string GetDeviceName(string path)
    {
        // Extraire le nom du device de "/DeviceName/control" ou "<DeviceName>/control"
        if (path.StartsWith("/"))
        {
            int secondSlash = path.IndexOf('/', 1);
            if (secondSlash > 0)
            {
                return path.Substring(1, secondSlash - 1);
            }
        }
        else if (path.StartsWith("<"))
        {
            int endBracket = path.IndexOf('>');
            if (endBracket > 0)
            {
                return path.Substring(1, endBracket - 1);
            }
        }
        return string.Empty;
    }

    private static string GetControlPart(string path)
    {
        // Extraire la partie contrôle "/control" de "/DeviceName/control"
        if (path.StartsWith("/"))
        {
            int secondSlash = path.IndexOf('/', 1);
            if (secondSlash > 0)
            {
                return path.Substring(secondSlash);
            }
        }
        else if (path.StartsWith("<"))
        {
            int endBracket = path.IndexOf('>');
            if (endBracket > 0)
            {
                return path.Substring(endBracket + 1);
            }
        }
        return path;
    }

    private static InputDeviceType DetectDeviceType(InputDevice device)
    {
        if (device is Keyboard || device is Mouse)
        {
            return InputDeviceType.KeyboardMouse;
        }

        if (device is Gamepad)
        {
            return InputDeviceType.Gamepad;
        }

        return InputDeviceType.KeyboardMouse;
    }

    private static bool IsIgnoredControl(InputControl control)
    {
        string path = control.path.ToLower();

        // Ignorer les contrôles de tracking/position (sauf sticks)
        if (path.Contains("position") && !path.Contains("stick")) return true;
        if (path.Contains("velocity")) return true;
        if (path.Contains("acceleration")) return true;
        if (path.Contains("angularvelocity")) return true;
        
        // Ignorer certains contrôles gamepad qui génèrent du bruit
        if (path.Contains("/touch")) return true; // Touchpad sur DualShock/DualSense
        
        // Ignorer les synthetic controls sauf pour certains cas utiles
        if (control.synthetic)
        {
            // Garder les sticks et dpad même s'ils sont synthetic
            if (path.Contains("stick") || path.Contains("dpad")) return false;
            return true;
        }

        return false;
    }

    private static string FormatBindingPath(string path)
    {
        // Convertir "/DeviceName/control" en "<DeviceName>/control"
        if (path.StartsWith("/"))
        {
            int secondSlash = path.IndexOf('/', 1);
            if (secondSlash > 0)
            {
                string deviceName = path.Substring(1, secondSlash - 1);
                string controlPath = path.Substring(secondSlash);
                return $"<{deviceName}>{controlPath}";
            }
        }
        return path;
    }
}
#endif
