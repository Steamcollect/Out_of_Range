using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomBaker : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToDisableForWall;
    [SerializeField] GameObject[] objectsToDisableForNavMesh;
    [SerializeField] NavMeshSurface surface;
    [SerializeField] RoomWallBaker wallBaker;

    [Button]
    void Bake()
    {
        NavMeshSurface[] surfaces = FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.InstanceID);
        foreach (var _surface in surfaces)
        {
            if(_surface != surface) _surface.gameObject.SetActive(false);
            else _surface.gameObject.SetActive(true);
        }

        foreach (var obj in objectsToDisableForWall)
            obj.gameObject.SetActive(false);

        foreach (var obj in objectsToDisableForNavMesh)
            obj.gameObject.SetActive(false);

        wallBaker.SetActiveWalls(false);
        surface.BuildNavMesh();
        wallBaker.Bake();

        foreach (var obj in objectsToDisableForWall)
            obj.SetActive(true);

        surface.BuildNavMesh();
        wallBaker.SetActiveWalls(true);

        foreach (var obj in objectsToDisableForNavMesh)
            obj.SetActive(true);

        foreach (var _surface in surfaces)
        {
            _surface.gameObject.SetActive(true);
        }

    }
}