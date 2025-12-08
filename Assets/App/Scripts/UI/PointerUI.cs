using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PointerUI : MonoBehaviour
{
    [FormerlySerializedAs("txt")] [SerializeField] private TMP_Text m_Txt;

    public void SetText(string text)
    {
        m_Txt.text = text;
    }
}