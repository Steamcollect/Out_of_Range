using MVsToolkit.Dev;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using DG.Tweening;

public class RadioAntennaTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, TagName] string playerTag;
    [SerializeField] string interactInput = "E";
    [SerializeField, Handle(TransformLocationType.Local)] Vector3 pointerPosition;

    [Space(10)]
    [SerializeField] float bumpScaleValue;
    [SerializeField] float bumpScaleTime;

    bool canPlayerInteract = false;
    bool isPlayerDetected = false;

    //[Header("References")]
    PointerUI currentPointer;

    [Header("Input")]
    [SerializeField] InputActionReference interactIA;

    //[Header("Output")]
    public Action OnPlayerInteract;

    private void OnEnable()
    {
        interactIA.action.started += OnInteractInput;
    }

    private void OnDisable()
    {
        interactIA.action.started -= OnInteractInput;
    }

    private void Update()
    {
        if(currentPointer != null && isPlayerDetected)
        {
            currentPointer.transform.position = Camera.main.WorldToScreenPoint(transform.position + pointerPosition);
        }
    }

    void OnInteractInput(InputAction.CallbackContext ctx)
    {
        if (!isPlayerDetected) return;

        OnPlayerInteract?.Invoke();
    }

    public void SetCanPlayerInteract(bool canPlayerInteract) { this.canPlayerInteract = canPlayerInteract; }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPlayerInteract) return;

        if (!isPlayerDetected && other.CompareTag(playerTag))
        {
            isPlayerDetected = true;
            currentPointer = InteractionUIManager.Instance.GetPointer();
            currentPointer.SetText(interactInput);

            currentPointer.transform.DOKill();
            currentPointer.transform.DOScale(bumpScaleValue, bumpScaleTime).SetLoops(2, LoopType.Yoyo);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerDetected && other.CompareTag(playerTag))
        {
            isPlayerDetected = false;
            InteractionUIManager.Instance.ReturnPointer(currentPointer);
        }
    }
}