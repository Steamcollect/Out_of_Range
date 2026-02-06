using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class RoomActivationManager : MonoBehaviour
{
    [SerializeField] GameObject[] rooms;
    [SerializeField] RSE_SetActiveRooms activeRooms;

    private void OnEnable()
    {
        activeRooms.Action += ActiveRooms;
    }

    private void OnDisable()
    {
        activeRooms.Action -= ActiveRooms;
    }

    void ActiveRooms(GameObject[] roomsToActive)
    {
        foreach (GameObject room in rooms)
        {
            if(roomsToActive.Contains(room)) room.SetActive(true);
            else room.SetActive(false);
        }
    }
}
