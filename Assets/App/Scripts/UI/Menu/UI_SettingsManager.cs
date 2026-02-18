using UnityEngine;

public class UI_SettingsManager : MonoBehaviour
{
        [Header("Inputs")]
        [SerializeField] private RSE_Reset m_ResetSettings;
        
        
        [Header("References")]
        [SerializeField] private SSO_UniversalSettings[] m_SettingsAsset;


	private void OnEnable()
        {
        	m_ResetSettings.Action += ResetAllSettings;
        }

	private void OnDisable()
        {
        	m_ResetSettings.Action -= ResetAllSettings;
        }

    private void ResetAllSettings()
    {
        foreach (SSO_UniversalSettings setting in m_SettingsAsset)
        {
            switch (setting.Type)
            {
                case SettingType.Float:
                    setting.SetNewFloatValue(setting.DefaultFloat);
                    break;
                case SettingType.Enum:
                    setting.SetNewEnumValue(setting.DefaultEnumIndex);
                    break;
            }
        }
    }
}