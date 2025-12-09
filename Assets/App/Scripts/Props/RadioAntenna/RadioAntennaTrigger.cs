using System;
using DG.Tweening;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class RadioAntennaTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [TagName] private string m_PlayerTag;
    [SerializeField] private string m_InteractInput = "E";
    [SerializeField] [Handle(TransformLocationType.Local)] private Vector3 m_PointerPosition;

    [Space(10)] 
    [SerializeField] private float m_BumpScaleValue;
    [SerializeField] private float m_BumpScaleTime;

    [Header("Input")]
    [SerializeField] private InputActionReference m_InteractIa;

    private bool m_CanPlayerInteract;

    //[Header("References")]
    private PointerUI m_CurrentPointer;
    private bool m_IsPlayerDetected;

    //[Header("Output")]
    public Action OnPlayerInteract;

    private void Start()
    {
        m_InteractIa.action.Enable();
    }

    private void Update()
    {
        if (m_CurrentPointer && m_IsPlayerDetected)
            m_CurrentPointer.transform.position = Camera.main.WorldToScreenPoint(transform.position + m_PointerPosition);
    }

    private void OnEnable()
    {
        m_InteractIa.action.started += OnInteractInput;
    }

    private void OnDisable()
    {
        m_InteractIa.action.started -= OnInteractInput;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_CanPlayerInteract) return;

        if (!m_IsPlayerDetected && other.CompareTag(m_PlayerTag))
        {
            m_IsPlayerDetected = true;
            m_CurrentPointer = InteractionUIManager.S_Instance.GetPointer();
            m_CurrentPointer.SetText(m_InteractInput);

            m_CurrentPointer.transform.DOKill();
            m_CurrentPointer.transform.DOScale(m_BumpScaleValue, m_BumpScaleTime).SetLoops(2, LoopType.Yoyo);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_IsPlayerDetected && other.CompareTag(m_PlayerTag))
        {
            m_IsPlayerDetected = false;
            InteractionUIManager.S_Instance.ReturnPointer(m_CurrentPointer);
        }
    }

    private void OnInteractInput(InputAction.CallbackContext ctx)
    {
        if (!m_IsPlayerDetected) return;

        OnPlayerInteract?.Invoke();
    }

    public void SetCanPlayerInteract(bool canPlayerInteract)
    {
        this.m_CanPlayerInteract = canPlayerInteract;
    }
}