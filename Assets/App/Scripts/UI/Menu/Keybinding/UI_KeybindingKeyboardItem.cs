using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UI_KeybindingKeyboardItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button m_SlotButton;

    [SerializeField] private TextMeshProUGUI m_ActionNameText;
    [SerializeField] private TextMeshProUGUI m_BindingText;
    [SerializeField] private Image m_BindingIcon;

    private UI_KeybindingKeyboardManager m_Manager;
    private UI_KeybindingKeyboardManager.ActionBinding m_ActionBinding;
    private InputAction m_InputAction;
    private int m_BindingIndex;

    public UI_KeybindingKeyboardManager.ActionBinding ActionBinding => m_ActionBinding;

    private void Awake()
    {
        if (m_SlotButton != null)
            m_SlotButton.onClick.AddListener(OnSlotClicked);
    }

    private void OnDestroy()
    {
        if (m_SlotButton != null)
            m_SlotButton.onClick.RemoveListener(OnSlotClicked);
    }

    public void Initialize(UI_KeybindingKeyboardManager manager,
        UI_KeybindingKeyboardManager.ActionBinding actionBinding,
        InputAction inputAction, int bindingIndex)
    {
        m_Manager = manager;
        m_ActionBinding = actionBinding;
        m_InputAction = inputAction;
        m_BindingIndex = bindingIndex;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (m_ActionBinding == null) return;

        if (m_ActionNameText != null)
            m_ActionNameText.text = m_ActionBinding.DisplayName;

        if (m_BindingText != null && m_InputAction != null && m_BindingIndex >= 0)
        {
            InputBinding binding = m_InputAction.bindings[m_BindingIndex];
            m_BindingText.text = UI_KeybindingKeyboardManager.GetKeyDisplayString(binding);
        }

        if (m_BindingIcon != null && m_ActionBinding.BindingIcon != null)
        {
            m_BindingIcon.sprite = m_ActionBinding.BindingIcon;
            m_BindingIcon.enabled = true;
        }
        else if (m_BindingIcon != null)
        {
            m_BindingIcon.enabled = false;
        }
    }

    private void OnSlotClicked()
    {
        if (m_Manager != null && m_InputAction != null)
            m_Manager.StartRebind(this, m_InputAction, m_BindingIndex);
    }

    public void SetInteractable(bool interactable)
    {
        if (m_SlotButton != null)
            m_SlotButton.interactable = interactable;
    }
}