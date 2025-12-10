using Sirenix.OdinInspector;
using UnityEngine;

public enum SettingType { Float, Enum }

[CreateAssetMenu(fileName = "UniversalSettings", menuName = "SSO/UniversalSettings")]
public class SSO_UniversalSettings : ScriptableObject
{
    [Header("Config")]
    public string ID;
    [EnumToggleButtons]
    public SettingType Type;

    [ShowIf("Type", SettingType.Float)] public float FloatValue = 1f;
    [ShowIf("Type", SettingType.Float)] public float MinFloat = 0f;
    [ShowIf("Type", SettingType.Float)] public float MaxFloat = 1f;

    [ShowIf("Type", SettingType.Enum)] public int EnumIndex = 0;
    [ShowIf("Type", SettingType.Enum)] public string[] EnumOptions;
}