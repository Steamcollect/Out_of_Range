using System;
using System.Diagnostics;

/// <summary>
/// Attribut pour marquer un champ string comme un Input Binding Path avec bouton Listen.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[Conditional("UNITY_EDITOR")]
public class InputBindingPathAttribute : Attribute
{
    /// <summary>
    /// Nom du champ DeviceType à mettre à jour automatiquement (optionnel).
    /// </summary>
    public string DeviceTypeFieldName { get; set; }

    public InputBindingPathAttribute()
    {
    }

    public InputBindingPathAttribute(string deviceTypeFieldName)
    {
        DeviceTypeFieldName = deviceTypeFieldName;
    }
}
