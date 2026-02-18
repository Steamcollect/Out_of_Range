using Sirenix.OdinInspector;
using UnityEngine;

public enum SettingType { Float, Enum }

[CreateAssetMenu(fileName = "UniversalSettings", menuName = "SSO/UniversalSettings")]
public class SSO_UniversalSettings : ScriptableObject
{
    [Title("CONFIGURATION")]
    public string ID;
    [EnumToggleButtons]
    public SettingType Type;

    [Title("Default Values")]
    [ShowIf("Type", SettingType.Float)] public float DefaultFloat = 1f;

    [ShowIf("Type", SettingType.Enum)] public int DefaultEnumIndex = 0;

    [ShowIf("Type", SettingType.Float)] public float MinFloat = 0f;
    [ShowIf("Type", SettingType.Float)] public float MaxFloat = 1f;

    [ShowIf("Type", SettingType.Enum)] public string[] EnumOptions;

    [Title("Runtime Values (Debug)")]
    [ReadOnly]
    [ShowIf("Type", SettingType.Float)] public float CurrentFloat;

    [ReadOnly]
    [ShowIf("Type", SettingType.Enum)] public int CurrentEnumIndex;

    public event System.Action<float> OnFloatChanged;
    public event System.Action<int> OnEnumChanged;

    public void SetNewFloatValue(float value)
    {
        CurrentFloat = value;
        PlayerPrefs.SetFloat(ID, CurrentFloat);
        PlayerPrefs.Save();

        OnFloatChanged?.Invoke(CurrentFloat);
    }

    public void SetNewEnumValue(int index)
    {
        CurrentEnumIndex = index;
        PlayerPrefs.SetInt(ID, CurrentEnumIndex);
        PlayerPrefs.Save();

        OnEnumChanged?.Invoke(CurrentEnumIndex);
    }

    public void LoadSavedValue()
    {
        switch(Type)
        {
            case SettingType.Float:
                CurrentFloat = PlayerPrefs.GetFloat(ID, DefaultFloat);
                OnFloatChanged?.Invoke(CurrentFloat);
                break;
            case SettingType.Enum:
                CurrentEnumIndex = PlayerPrefs.GetInt(ID, DefaultEnumIndex);
                OnEnumChanged?.Invoke(CurrentEnumIndex);
                break;
        }  
    }

    [Button("Reset To Default")]
    public void ResetToDefault()
    {
        switch (Type)
        {
            case SettingType.Float:
                SetNewFloatValue(DefaultFloat);
                break;
            case SettingType.Enum:
                SetNewEnumValue(DefaultEnumIndex);
                break;
        }
    }
}