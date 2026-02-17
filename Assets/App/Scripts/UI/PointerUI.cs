using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PointerUI : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private InputActionIconDisplay m_InputActionIconDisplay;

    public void Set(InputActionReference inputActionReference)
    {
        m_InputActionIconDisplay.SetAction(inputActionReference);
    }
}