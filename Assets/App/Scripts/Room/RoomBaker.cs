using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomBaker : MonoBehaviour
{
    [SerializeField, Tooltip("Objets qui ne seront pas calculé par les mur invisible")] GameObject[] navMeshObjects;
    [SerializeField, Tooltip("Objets qui ne seront pas calculé par le navmesh surface")] GameObject[] wallsObjects;

    [SerializeField] bool m_BakeOnlyRoom = false;
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

        foreach (var obj in navMeshObjects)
            obj.gameObject.SetActive(false);

        foreach (var obj in wallsObjects)
            obj.gameObject.SetActive(false);

        wallBaker.SetActiveWalls(false);
        surface.BuildNavMesh();
        if(!m_BakeOnlyRoom) wallBaker.Bake(surface);

        foreach (var obj in navMeshObjects)
            obj.SetActive(true);

        surface.BuildNavMesh();
        wallBaker.SetActiveWalls(true);

        foreach (var obj in wallsObjects)
            obj.SetActive(true);

        foreach (var _surface in surfaces)
        {
            _surface.gameObject.SetActive(true);
        }
    }
}