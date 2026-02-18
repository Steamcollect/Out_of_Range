using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CustomDropdown : MonoBehaviour, IPointerExitHandler
{
    [Title("SETTINGS")]
    [SerializeField] private bool m_EnableIcon = true;
    [SerializeField] private bool m_InvokeAtStart = false;
    [Title("SAVING")]
    [SerializeField] private string m_DropdownTag = "Dropdown";
    [Title("CONTENT")]
    public List<Item> DropdownItems = new List<Item>();
    public int SelectedIndex = 0;

    [Title("REFERENCES")]
    [SerializeField] private GameObject m_TriggerObject;
    [SerializeField] private TextMeshProUGUI m_SelectedText;
    [SerializeField] private Image m_SelectedImage;
    [SerializeField] private Transform m_ItemParent;
    [SerializeField] private GameObject m_ItemObject;
    [SerializeField] private GameObject m_Scrollbar;
    [SerializeField] private Transform m_ListParent;
    [SerializeField] private VerticalLayoutGroup m_ItemList;
    [SerializeField] private Animator m_DropdownAnimator;

    public bool m_IsOn;
    [HideInInspector] public int Index = 0;
    [HideInInspector] public int SiblingIndex = 0;

    private Transform m_CurrentListParent;
    private TextMeshProUGUI m_SetItemText;
    private Image m_SetItemImage;

    private Sprite m_ImageHelper;
    private string m_TextHelper;
    private string m_NewItemTitle;
    private Sprite m_NewItemIcon;

    [System.Serializable]
    public class Item
    {
        public string ItemName = "Dropdown Item";
        public Sprite ItemIcon;
        public UnityEvent OnItemSelection;
    }

    private void Start()
    {
        SetupDropdown();
        m_CurrentListParent = transform.parent;

        if(m_EnableIcon == false)
            m_SelectedImage.gameObject.SetActive(false);
        else
            m_SelectedImage.gameObject.SetActive(true);

        m_ItemList.padding.right = 25;
        m_Scrollbar.SetActive(true);

        transform.SetAsLastSibling();

        if (m_InvokeAtStart == true)
            DropdownItems[PlayerPrefs.GetInt(m_DropdownTag + "Dropdown")].OnItemSelection.Invoke();
        else
            ChangeDropdownInfo(PlayerPrefs.GetInt(m_DropdownTag + "Dropdown"));
    }

    public void SetupDropdown()
    {
        foreach(Transform child in m_ItemParent)
        {
            Destroy(child.gameObject);
        }

        Index = 0;
        for(int i = 0; i < DropdownItems.Count; i++)
        {
            GameObject go = Instantiate(m_ItemObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(m_ItemParent, false);

            m_SetItemText = go.GetComponentInChildren<TextMeshProUGUI>();
            m_TextHelper = DropdownItems[i].ItemName;
            m_SetItemText.text = m_TextHelper;

            Transform goImage;
            goImage = go.gameObject.transform.Find("Icon");
            m_SetItemImage = goImage.GetComponent<Image>();
            m_ImageHelper = DropdownItems[i].ItemIcon;
            m_SetItemImage.sprite = m_ImageHelper;

            Button itemButton;
            itemButton = go.GetComponent<Button>();

            if (DropdownItems[i].OnItemSelection != null)
                itemButton.onClick.AddListener(DropdownItems[i].OnItemSelection.Invoke);

            itemButton.onClick.AddListener(Animate);
            itemButton.onClick.AddListener(delegate
            {
                ChangeDropdownInfo(Index = go.transform.GetSiblingIndex());

                PlayerPrefs.SetInt(m_DropdownTag + "Dropdown", go.transform.GetSiblingIndex());
            });

            if(m_InvokeAtStart == true)
                DropdownItems[i].OnItemSelection.Invoke();
        }

        m_SelectedText.text = DropdownItems[SelectedIndex].ItemName;
        m_SelectedImage.sprite = DropdownItems[SelectedIndex].ItemIcon;
        m_CurrentListParent = transform.parent;
    }

    public void ChangeDropdownInfo(int itemIndex)
    {
        m_SelectedImage.sprite = DropdownItems[itemIndex].ItemIcon;
        m_SelectedText.text = DropdownItems[itemIndex].ItemName;
        SelectedIndex = itemIndex;
    }

    public void Animate()
    {
        StartCoroutine(AnimateDropdown());
    }

    private IEnumerator AnimateDropdown()
    {
        if (!m_IsOn)
        {
            StopAllCoroutines();
            m_DropdownAnimator.Play("Dropdown_In");
            m_IsOn = true;

            SiblingIndex = transform.GetSiblingIndex();
            gameObject.transform.SetParent(m_ListParent, true);
            transform.SetAsLastSibling();
        }
        else
        {
            m_DropdownAnimator.Play("Dropdown_Out");
            m_IsOn = false;

            yield return new WaitForSeconds(.15f);
            Debug.Log("after 1 second");

            gameObject.transform.SetParent(m_CurrentListParent, true);
            gameObject.transform.SetSiblingIndex(SiblingIndex);
            transform.SetAsLastSibling();
        }

        if (m_IsOn == false)
            m_TriggerObject.SetActive(false);
        else if (m_IsOn == true)
            m_TriggerObject.SetActive(true);

        m_TriggerObject.SetActive(false);

        yield return null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_IsOn == true)
        {
            Animate();
        }
    }

    public void UpdateValues()
    {
        m_ItemList.padding.right = 25;
        m_Scrollbar.SetActive(true);

        if (m_EnableIcon == false)
            m_SelectedImage.gameObject.SetActive(false);
        else
            m_SelectedImage.gameObject.SetActive(true);
    }

    public void CreateNewItem()
    {
        Item item = new Item();
        item.ItemName = m_NewItemTitle;
        item.ItemIcon = m_NewItemIcon;
        DropdownItems.Add(item);
        SetupDropdown();
    }

    public void CreateNewOption(string title)
    {
        Item item = new Item();
        item.ItemName = title;
        DropdownItems.Add(item);
    }

    public void SetItemTitle(string title)
    {
        m_NewItemTitle = title;
    }

    public void SetItemIcon(Sprite icon)
    {
        m_NewItemIcon = icon;
    }
}