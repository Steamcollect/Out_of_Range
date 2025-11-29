using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float openTime;
    [SerializeField] DoorStartType type;
    public enum DoorStartType { Open, Close, Default };

    [Header("References")]
    [SerializeField] Transform openPoint;
    [SerializeField] Transform closePoint;
    Vector3 openPos; 
    Vector3 closePos;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        openPos = openPoint.position;
        closePos = closePoint.position;

        switch (type)
        {
            case DoorStartType.Open:
                transform.position = openPos;
                break;
            case DoorStartType.Close:
                transform.position = closePos;
                break;
        }
    }

    public void OpenDoor()
    {
        transform.DOKill();
        transform.DOMove(openPos, openTime);
    }
    public void CloseDoor()
    {
        transform.DOKill();
        transform.DOMove(closePos, openTime);
    }
}