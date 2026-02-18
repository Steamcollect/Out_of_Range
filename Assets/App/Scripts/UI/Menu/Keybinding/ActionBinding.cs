using System;
using UnityEngine;

[Serializable]
public class ActionBinding
{
    [Tooltip("Name of the action in the InputActionMap")]
    public string ActionName;
        
    [Tooltip("Display name shown in the UI")]
    public string DisplayName;
        
    [Tooltip("Category for grouping actions in the UI")]
    public string Category = "General";
        
    [Tooltip("The keyboard/mouse binding path")]
    public string BindingPath;
        
    [Tooltip("Icon to display for this binding")]
    public Sprite BindingIcon;
}