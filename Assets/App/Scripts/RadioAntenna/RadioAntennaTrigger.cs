using MVsToolkit.Dev;
using Unity.VisualScripting;
using UnityEngine;

public class RadioAntennaTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, TagName] string playerTag;
    [SerializeField] string interactInput = "E";
    [SerializeField, Handle(TransformLocationType.Local)] Vector3 pointerPosition;

    bool isPlayerDetected = false;

    //[Header("References")]
    PointerUI currentPointer;

    [Header("Input")]
    [SerializeField] RSO_MainCamera cam;

    //[Header("Output")]

    private void Update()
    {
        if(currentPointer != null && isPlayerDetected)
        {
            currentPointer.transform.position = cam.Get().GetCamera().WorldToScreenPoint(transform.position + pointerPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayerDetected && other.CompareTag(playerTag))
        {
            isPlayerDetected = true;
            currentPointer = InteractionUIManager.Instance.CreatePointerUI();
            currentPointer.SetText(interactInput);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerDetected && other.CompareTag(playerTag))
        {
            isPlayerDetected = false;
            Destroy(currentPointer.gameObject);
        }
    }
}