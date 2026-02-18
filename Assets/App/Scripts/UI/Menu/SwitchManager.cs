using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class SwitchManager : MonoBehaviour
{
    [Title("Settings")]
    [Tooltip("Every switch must have a different tag")]
    [SerializeField] private string m_SwitchTag = "Switch";
    [SerializeField] private bool m_IsOn = true;
    [SerializeField] private bool m_SaveValue = true;
    [SerializeField] private bool m_InvokeAtStart = true;
    [Space(10)]
    [SerializeField] private UnityEvent m_OnEvents;
    [SerializeField] private UnityEvent m_OffEvents;

    [Title("References")]
    [SerializeField] private Animator m_SwitchAnimator;
    [SerializeField] private Button m_SwitchButton;

    private void Start()
    {
        m_SwitchButton.onClick.AddListener(AnimateSwitch);

        if (m_SaveValue == true)
        {
            if (PlayerPrefs.GetString(m_SwitchTag + "Switch") == "")
            {
                if (m_IsOn == true)
                {
                    m_SwitchAnimator.Play("Switch On");
                    m_IsOn = true;
                    PlayerPrefs.SetString(m_SwitchTag + "Switch", "true");
                }

                else
                {
                    m_SwitchAnimator.Play("Switch Off");
                    m_IsOn = false;
                    PlayerPrefs.SetString(m_SwitchTag + "Switch", "false");
                }
            }

            else if (PlayerPrefs.GetString(m_SwitchTag + "Switch") == "true")
            {
                m_SwitchAnimator.Play("Switch On");
                m_IsOn = true;
            }

            else if (PlayerPrefs.GetString(m_SwitchTag + "Switch") == "false")
            {
                m_SwitchAnimator.Play("Switch Off");
                m_IsOn = false;
            }
        }

        else
        {
            if (m_IsOn == true)
            {
                m_SwitchAnimator.Play("Switch On");
                m_IsOn = true;
            }

            else
            {
                m_SwitchAnimator.Play("Switch Off");
                m_IsOn = false;
            }
        }

        if (m_InvokeAtStart == true && m_IsOn == true)
            m_OnEvents.Invoke();
        if (m_InvokeAtStart == true && m_IsOn == false)
            m_OffEvents.Invoke();
    }

    public void AnimateSwitch()
    {
        if (m_IsOn == true)
        {
            m_SwitchAnimator.Play("Switch Off");
            m_IsOn = false;
            m_OffEvents.Invoke();

            if (m_SaveValue == true)
                PlayerPrefs.SetString(m_SwitchTag + "Switch", "false");
        }

        else
        {
            m_SwitchAnimator.Play("Switch On");
            m_IsOn = true;
            m_OnEvents.Invoke();

            if (m_SaveValue == true)
                PlayerPrefs.SetString(m_SwitchTag + "Switch", "true");
        }
    }
}