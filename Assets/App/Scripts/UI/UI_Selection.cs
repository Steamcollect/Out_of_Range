using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Selection : MonoBehaviour
{
    [Title("REFERENCES")]
    [SerializeField] private SSO_UniversalSettings m_Data;
    [ShowIf("@m_Data.Type", SettingType.Enum)]
    [SerializeField] private Button m_LeftArrow;
    [ShowIf("@m_Data.Type", SettingType.Enum)]
    [SerializeField] private Button m_RightArrow;
    [ShowIf("@m_Data.Type", SettingType.Enum)]
    [SerializeField] private TextMeshProUGUI m_SelectedText;

    [ShowIf("@m_Data.Type", SettingType.Enum)] 
    public int SelectedIndex = 0;

    [ShowIf("@m_Data.Type", SettingType.Float)]
    [SerializeField] private Slider m_Slider;

    [ShowIf("@m_Data.Type", SettingType.Float)]
    [SerializeField] private TextMeshProUGUI m_PreviewValue;

    private void Start()
    {
        switch (m_Data.Type) 
        {
            case SettingType.Float:
                m_Slider.minValue = m_Data.MinFloat;
                m_Slider.maxValue = m_Data.MaxFloat;
                m_Slider.value = PlayerPrefs.GetFloat(m_Data.ID, m_Data.FloatValue);

                UpdateValue(m_Slider.value);

                m_Slider.onValueChanged.AddListener(UpdateValue);
                
                break;
            case SettingType.Enum:
                SelectedIndex = PlayerPrefs.GetInt(m_Data.ID, m_Data.EnumIndex);

                if (m_Data.EnumOptions.Length != 0) UpdateDisplay();

                m_LeftArrow.onClick.AddListener(SelectPrevious);
                m_RightArrow.onClick.AddListener(SelectNext);
                break;
        }
    }

    private void UpdateValue(float value)
    {
        PlayerPrefs.SetFloat(m_Data.ID, value);
        m_PreviewValue.text = value.ToString("F2");
    }

    private void SelectPrevious()
    {
        if (SelectedIndex <= 0) return;
        SelectedIndex--;
        InvokeItemEvent();
    }

    private void SelectNext()
    {
        if (SelectedIndex >= m_Data.EnumOptions.Length - 1) return;
        SelectedIndex++;
        InvokeItemEvent();
    }

    public void InvokeItemEvent()
    {
        PlayerPrefs.SetInt(m_Data.ID, SelectedIndex);

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        m_SelectedText.text = m_Data.EnumOptions[SelectedIndex];

        m_LeftArrow.interactable = SelectedIndex > 0 ? true : false;
        m_RightArrow.interactable = SelectedIndex < m_Data.EnumOptions.Length - 1 ? true : false;
    }
}