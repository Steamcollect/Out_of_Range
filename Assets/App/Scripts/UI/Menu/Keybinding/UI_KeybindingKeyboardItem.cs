using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UI_KeybindingKeyboardItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI m_ActionNameText;

    [Header("Primary Binding")]
    [SerializeField] private Button m_PrimarySlotButton;
    [SerializeField] private TextMeshProUGUI m_PrimaryBindingText;
    [SerializeField] private Image m_PrimaryBindingIcon;

    [Header("Secondary Binding")]
    [SerializeField] private Button m_SecondarySlotButton;
    [SerializeField] private TextMeshProUGUI m_SecondaryBindingText;
    [SerializeField] private Image m_SecondaryBindingIcon;

    [Header("Display Settings")]
    [SerializeField] private string m_EmptyBindingText = "---";

    private UI_KeybindingManager m_Manager;
    private UI_KeybindingManager.ActionBinding m_ActionBinding;
    private InputAction m_InputAction;
    private int m_PrimaryBindingIndex = -1;
    private int m_SecondaryBindingIndex = -1;

    public UI_KeybindingManager.ActionBinding ActionBinding => m_ActionBinding;
    public int PrimaryBindingIndex => m_PrimaryBindingIndex;
    public int SecondaryBindingIndex => m_SecondaryBindingIndex;

    private void Awake()
    {
        if (m_PrimarySlotButton != null)
            m_PrimarySlotButton.onClick.AddListener(OnPrimarySlotClicked);
        if (m_SecondarySlotButton != null)
            m_SecondarySlotButton.onClick.AddListener(OnSecondarySlotClicked);
    }

    private void OnDestroy()
    {
        if (m_PrimarySlotButton != null)
            m_PrimarySlotButton.onClick.RemoveListener(OnPrimarySlotClicked);
        if (m_SecondarySlotButton != null)
            m_SecondarySlotButton.onClick.RemoveListener(OnSecondarySlotClicked);
    }

    public void Initialize(UI_KeybindingManager manager,
        UI_KeybindingManager.ActionBinding actionBinding,
        InputAction inputAction,
        int primaryBindingIndex,
        int secondaryBindingIndex = -1)
    {
        m_Manager = manager;
        m_ActionBinding = actionBinding;
        m_InputAction = inputAction;
        m_PrimaryBindingIndex = primaryBindingIndex;
        m_SecondaryBindingIndex = secondaryBindingIndex;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (m_ActionBinding == null) return;

        if (m_ActionNameText != null)
            m_ActionNameText.text = m_ActionBinding.DisplayName;

        UpdateSlotDisplay(m_PrimaryBindingText, m_PrimaryBindingIcon, m_PrimaryBindingIndex);
        UpdateSlotDisplay(m_SecondaryBindingText, m_SecondaryBindingIcon, m_SecondaryBindingIndex);

        if (m_SecondarySlotButton != null)
        {
            // Afficher le slot secondaire si HasSecondarySlot est true OU si un binding secondaire existe déjà
            bool showSecondary = m_ActionBinding.HasSecondarySlot || m_SecondaryBindingIndex >= 0;
            m_SecondarySlotButton.gameObject.SetActive(showSecondary);
        }
    }

    private void UpdateSlotDisplay(TextMeshProUGUI bindingText, Image bindingIcon, int bindingIndex)
    {
        if (bindingText == null) return;

        if (m_InputAction == null || bindingIndex < 0)
        {
            bindingText.text = m_EmptyBindingText;
            if (bindingIcon != null)
                bindingIcon.enabled = false;
            return;
        }

        InputBinding binding = m_InputAction.bindings[bindingIndex];
        
        if (string.IsNullOrEmpty(binding.effectivePath))
        {
            bindingText.text = m_EmptyBindingText;
            if (bindingIcon != null)
                bindingIcon.enabled = false;
            return;
        }

        bindingText.text = UI_KeybindingManager.GetKeyDisplayString(binding);

        if (bindingIcon != null && m_ActionBinding?.BindingIcon != null)
        {
            bindingIcon.sprite = m_ActionBinding.BindingIcon;
            bindingIcon.enabled = true;
        }
        else if (bindingIcon != null)
        {
            bindingIcon.enabled = false;
        }
    }

    private void OnPrimarySlotClicked()
    {
        if (m_Manager != null && m_InputAction != null && m_PrimaryBindingIndex >= 0)
            m_Manager.StartRebind(this, m_InputAction, m_PrimaryBindingIndex, 
                m_ActionBinding?.DisplayName ?? m_InputAction.name, true);
    }

    private void OnSecondarySlotClicked()
    {
        if (m_Manager == null || m_InputAction == null) return;
        
        // Si pas de binding secondaire existant, demander au manager de créer un nouveau binding
        if (m_SecondaryBindingIndex < 0)
        {
            m_Manager.StartRebindNewBinding(this, m_InputAction, 
                m_ActionBinding?.DisplayName ?? m_InputAction.name, false);
        }
        else
        {
            m_Manager.StartRebind(this, m_InputAction, m_SecondaryBindingIndex,
                m_ActionBinding?.DisplayName ?? m_InputAction.name, false);
        }
    }

    public void SetInteractable(bool interactable)
    {
        if (m_PrimarySlotButton != null)
            m_PrimarySlotButton.interactable = interactable;
        if (m_SecondarySlotButton != null)
            m_SecondarySlotButton.interactable = interactable;
    }

    public void ClearBinding(int bindingIndex)
    {
        if (m_InputAction == null || bindingIndex < 0) return;
        m_InputAction.ApplyBindingOverride(bindingIndex, "");
        UpdateDisplay();
    }

    public void SetSecondaryBindingIndex(int index)
    {
        m_SecondaryBindingIndex = index;
        UpdateDisplay();
    }
}