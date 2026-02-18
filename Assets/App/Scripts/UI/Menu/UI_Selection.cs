using System;
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

    [ShowIf("@m_Data.Type", SettingType.Float)]
    [SerializeField] private Slider m_Slider;

    [ShowIf("@m_Data.Type", SettingType.Float)]
    [SerializeField] private TextMeshProUGUI m_PreviewValue;

    private void Awake()
    {
        switch (m_Data.Type) 
        {
            case SettingType.Float:
                m_Slider.minValue = m_Data.MinFloat;
                m_Slider.maxValue = m_Data.MaxFloat;
                m_Slider.value = m_Data.CurrentFloat;

                m_Slider.onValueChanged.AddListener((value) => {
                    m_Data.SetNewFloatValue(value);
                    UpdatePreviewText(value);
                });
                break;
            case SettingType.Enum:
                m_LeftArrow.onClick.AddListener(() => ChangeEnum(-1));
                m_RightArrow.onClick.AddListener(() => ChangeEnum(1));
                break;
        }
    }

    private void Start()
    {
        switch (m_Data.Type)
        {
            case SettingType.Float:
                UpdatePreviewText(m_Data.CurrentFloat);
                break;
            case SettingType.Enum:
                UpdateEnumDisplay();
                break;
        }
    }

    private void OnEnable()
    {
        switch (m_Data.Type)
        {
            case SettingType.Float:
                m_Data.OnFloatChanged += UpdateRenderFloat;
                break;
            case SettingType.Enum:
                m_Data.OnEnumChanged += UpdateRenderEnum;
                break;
        }
    }

    private void OnDisable()
    {
        switch (m_Data.Type)
        {
            case SettingType.Float:
                m_Data.OnFloatChanged -= UpdateRenderFloat;
                break;
            case SettingType.Enum:
                m_Data.OnEnumChanged -= UpdateRenderEnum;
                break;
        }
    }

    
    private void UpdateRenderFloat(float f)
    {
        m_Slider.value = f;
        UpdatePreviewText(f);
    }
    
    private void UpdateRenderEnum(int i) => UpdateEnumDisplay();


    private void ChangeEnum(int direction)
    {
        int newIndex = m_Data.CurrentEnumIndex + direction;

        if (newIndex < 0 || newIndex >= m_Data.EnumOptions.Length) return;

        m_Data.SetNewEnumValue(newIndex);
        UpdateEnumDisplay();
    }

    private void UpdatePreviewText(float value)
    {
        m_PreviewValue.text = value.ToString("F2");
    }
    
    private void UpdateEnumDisplay()
    {
        if(m_Data.EnumOptions.Length > 0)
            m_SelectedText.text = m_Data.EnumOptions[m_Data.CurrentEnumIndex];

        m_LeftArrow.interactable = m_Data.CurrentEnumIndex > 0;
        m_RightArrow.interactable = m_Data.CurrentEnumIndex < m_Data.EnumOptions.Length - 1;
    }
}