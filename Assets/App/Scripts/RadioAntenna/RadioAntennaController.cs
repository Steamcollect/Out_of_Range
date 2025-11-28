using UnityEngine;
using UnityEngine.Events;

public class RadioAntennaController : MonoBehaviour
{
    bool asInteract = false;

    //[Header("References")]

    [Header("References")]
    [SerializeField] RadioAntennaTrigger trigger;

    [Header("Output")]
    [SerializeField] UnityEvent _OnPlayerInteract;

    private void OnEnable()
    {
        trigger.OnPlayerInteract += OnPlayerInteract;
    }
    private void OnDisable()
    {
        trigger.OnPlayerInteract -= OnPlayerInteract;
    }

    private void Start()
    {
        asInteract = false;
        trigger.SetCanPlayerInteract(true);
    }

    void OnPlayerInteract()
    {
        if (asInteract) return;

        asInteract = true;
        _OnPlayerInteract.Invoke();
    }
}