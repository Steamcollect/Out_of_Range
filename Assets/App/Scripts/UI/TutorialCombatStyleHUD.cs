using UnityEngine;

public class TutorialCombatStyleHUD : BaseCombatStyleHUD
{
    [Header("Tutorial References")]
    [SerializeField] private FakeOverloadCombatStyle m_FakeOverloadStyle;

    private void OnEnable()
    {
        BindToFakeCombatStyle(m_FakeOverloadStyle);
    }

    private void OnDisable()
    {
        UnbindFromFakeCombatStyle();
    }

    private void BindToFakeCombatStyle(FakeOverloadCombatStyle fakeStyle)
    {
        UnbindFromFakeCombatStyle();

        m_FakeOverloadStyle = fakeStyle;

        m_FakeOverloadStyle.OnAmmoChange += SetFillValue;
        m_FakeOverloadStyle.OnOverloadStart += EnableReloadSkills;
        m_FakeOverloadStyle.OnOverloadEnd += DisableReloadSkills;
        m_FakeOverloadStyle.OnOverloadStateChange += OnOverloadStateChange;

        SetReloadSkillsRectFromFake();
    }

    private void UnbindFromFakeCombatStyle()
    {
        m_FakeOverloadStyle.OnAmmoChange -= SetFillValue;
        m_FakeOverloadStyle.OnOverloadStart -= EnableReloadSkills;
        m_FakeOverloadStyle.OnOverloadEnd -= DisableReloadSkills;
        m_FakeOverloadStyle.OnOverloadStateChange -= OnOverloadStateChange;
    }

    private void SetReloadSkillsRectFromFake()
    {

        Vector2 buffValues = m_FakeOverloadStyle.GetRangeToBuff();
        Vector2 resetValues = m_FakeOverloadStyle.GetRangeToReset();
        m_ParentWidth = m_ParentRect.rect.width;

        if (m_BuffZoneRct != null)
        {
            float left = buffValues.x * (m_ParentWidth * .01f);
            float right = buffValues.y * (m_ParentWidth * .01f);

            Vector2 offMin = m_BuffZoneRct.offsetMin;
            offMin.x = left;
            m_BuffZoneRct.offsetMin = offMin;

            Vector2 offMax = m_BuffZoneRct.offsetMax;
            offMax.x = -(m_ParentWidth - right);
            m_BuffZoneRct.offsetMax = offMax;
        }

        if (m_ResetZoneRct != null)
        {
            float left = resetValues.x * (m_ParentWidth * .01f);
            float right = resetValues.y * (m_ParentWidth * .01f);

            Vector2 offMin = m_ResetZoneRct.offsetMin;
            offMin.x = left;
            m_ResetZoneRct.offsetMin = offMin;

            Vector2 offMax = m_ResetZoneRct.offsetMax;
            offMax.x = -(m_ParentWidth - right);
            m_ResetZoneRct.offsetMax = offMax;
        }
    }

    protected override void SetFillValue(float value, float max)
    {
        float normalizedValue = Mathf.Clamp(value, 0, max) / max;
        m_FillImg.fillAmount = normalizedValue;

        m_CursorRct.anchoredPosition = new Vector2(
            (normalizedValue * m_ParentWidth),
            m_CursorRct.anchoredPosition.y);


        UpdateVisualsByState(m_FakeOverloadStyle.GetState(), normalizedValue);
    }
}

