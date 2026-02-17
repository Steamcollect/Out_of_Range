using System;
using UnityEngine;

/// <summary>
/// Représente une entrée d'icône pour un binding path spécifique.
/// </summary>
[Serializable]
public struct InputBindingIconEntry
{
    [Tooltip("Le binding path exact Unity Input System (ex: '<Keyboard>/z', '<Gamepad>/leftStick')")]
    public string BindingPath;

    [Tooltip("Le type de périphérique associé à cette icône")]
    public InputDeviceType DeviceType;

    [Tooltip("L'icône Sprite à afficher pour ce binding")]
    public Sprite Icon;

    [Tooltip("Label optionnel personnalisé (si vide, sera auto-généré depuis le path)")]
    public string CustomDisplayName;
}