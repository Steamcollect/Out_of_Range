using System;
using UnityEngine;

public class ColliderCallback : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public Action<Collider> OnTriggerEnterCallback;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterCallback?.Invoke(other);
    }
}