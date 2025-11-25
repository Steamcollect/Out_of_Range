using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] Camera cam;

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] RSO_MainCamera rso_MainCamera;

    void Awake()
    {
        rso_MainCamera.Set(cam);
    }

    public Camera GetCamera() { return cam; }
}