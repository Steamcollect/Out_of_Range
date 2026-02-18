using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Title("REFERENCES")]
    [SerializeField] private Animator m_ButtonAnimator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_ButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("HoverToPressed"))
            m_ButtonAnimator.Play("NormalToHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!m_ButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("HoverToPressed"))
            m_ButtonAnimator.Play("HoverToNormal");
    }
}