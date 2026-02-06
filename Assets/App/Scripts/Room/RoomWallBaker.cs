using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomWallBaker : MonoBehaviour
{
    NavMeshSurface surface;

    [Header("Wall Settings")]
    public float wallHeight = 5f;
    public float wallThickness = 1;
    public float wallOffset = 1;

    [Header("Layer")]
    public int wallLayer = 13;

    [Header("Hierarchy")]
    public string wallsRootName = "GeneratedWalls";

    [Header("Debug")]
    public bool showGizmos = true;
    private List<Vector3> outerLoop = new List<Vector3>();

    private Transform wallsRoot;

    [Button]
    public void Bake(NavMeshSurface surface)
    {
        this.surface = surface;

        PrepareWallsRoot();
        ClearWalls();

        surface.BuildNavMesh();

        var tri = NavMesh.CalculateTriangulation();

        // 1) Snap des vertices pour éviter les doublons
        var snappedVerts = tri.vertices.Select(v => Snap(v, 0.01f)).ToArray();

        // 2) Extraction des edges frontières
        var borderEdges = ExtractBorderEdges(tri.indices, snappedVerts);

        // 3) Reconstruction de toutes les boucles
        var loops = BuildLoops(borderEdges);

        if (loops.Count == 0)
        {
            Debug.LogError("Aucune boucle trouvée.");
            return;
        }

        // 4) Générer des murs pour toutes les boucles extérieures
        foreach (var loop in loops)
        {
            float area = PolygonAreaXZ(loop);

            // Ignore les trous internes minuscules
            if (area < 0.5f)
                continue;

            BuildWalls(loop);
        }

        // Pour le debug, on garde la plus grande boucle
        outerLoop = loops.OrderByDescending(PolygonAreaXZ).First();

        Debug.Log($"RoomWallBaker : {outerLoop.Count} points dans la boucle extérieure.");
    }

    public void SetActiveWalls(bool state)
    {
        if(wallsRoot != null)
        {
            wallsRoot.gameObject.SetActive(state);
        }
    }

    private void PrepareWallsRoot()
    {
        if (wallsRoot != null)
            return;

        var existing = transform.Find(wallsRootName);
        if (existing != null)
        {
            wallsRoot = existing;
            return;
        }

        var go = new GameObject(wallsRootName);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        wallsRoot = go.transform;
    }

    private Vector3 Snap(Vector3 v, float precision)
    {
        return new Vector3(
            Mathf.Round(v.x / precision) * precision,
            Mathf.Round(v.y / precision) * precision,
            Mathf.Round(v.z / precision) * precision
        );
    }

    private List<(Vector3 a, Vector3 b)> ExtractBorderEdges(int[] indices, Vector3[] verts)
    {
        var edgeCount = new Dictionary<(Vector3, Vector3), int>();

        void AddEdge(Vector3 v1, Vector3 v2)
        {
            var key = v1.x < v2.x || (Mathf.Approximately(v1.x, v2.x) && v1.z < v2.z)
                ? (v1, v2)
                : (v2, v1);

            if (!edgeCount.ContainsKey(key))
                edgeCount[key] = 0;

            edgeCount[key]++;
        }

        for (int i = 0; i < indices.Length; i += 3)
        {
            var a = verts[indices[i]];
            var b = verts[indices[i + 1]];
            var c = verts[indices[i + 2]];

            AddEdge(a, b);
            AddEdge(b, c);
            AddEdge(c, a);
        }

        // On garde uniquement les edges utilisés par un seul triangle → frontière
        return edgeCount.Where(kvp => kvp.Value == 1)
                        .Select(kvp => kvp.Key)
                        .ToList();
    }

    private List<List<Vector3>> BuildLoops(List<(Vector3 a, Vector3 b)> edges)
    {
        var adjacency = new Dictionary<Vector3, List<Vector3>>();

        foreach (var e in edges)
        {
            if (!adjacency.ContainsKey(e.a)) adjacency[e.a] = new List<Vector3>();
            if (!adjacency.ContainsKey(e.b)) adjacency[e.b] = new List<Vector3>();

            adjacency[e.a].Add(e.b);
            adjacency[e.b].Add(e.a);
        }

        var loops = new List<List<Vector3>>();
        var visitedVertices = new HashSet<Vector3>();

        foreach (var start in adjacency.Keys)
        {
            if (visitedVertices.Contains(start))
                continue;

            // On démarre une nouvelle boucle à partir de ce sommet
            var neighbors = adjacency[start];
            if (neighbors.Count == 0)
                continue;

            var loop = new List<Vector3>();
            Vector3 current = start;
            Vector3 previous = neighbors[0]; // on prend un voisin arbitraire

            loop.Add(current);
            visitedVertices.Add(current);

            int safety = 0;
            while (safety++ < 10000)
            {
                loop.Add(previous);
                visitedVertices.Add(previous);

                var nextNeighbors = adjacency[previous];
                Vector3 nextCandidate = nextNeighbors.FirstOrDefault(n => n != current);

                if (nextCandidate == start)
                {
                    // boucle fermée
                    loops.Add(loop);
                    break;
                }

                current = previous;
                previous = nextCandidate;
            }
        }

        return loops;
    }

    private float SignedPolygonAreaXZ(List<Vector3> pts)
    {
        float area = 0f;
        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 a = pts[i];
            Vector3 b = pts[(i + 1) % pts.Count];
            area += (a.x * b.z - b.x * a.z);
        }
        return area * 0.5f;
    }


    private float PolygonAreaXZ(List<Vector3> pts)
    {
        float area = 0f;
        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 a = pts[i];
            Vector3 b = pts[(i + 1) % pts.Count];
            area += (a.x * b.z - b.x * a.z);
        }
        return Mathf.Abs(area) * 0.5f;
    }

    private void BuildWalls(List<Vector3> loop)
    {
        for (int i = 0; i < loop.Count; i++)
        {
            Vector3 p1 = loop[i];
            Vector3 p2 = loop[(i + 1) % loop.Count];

            // Décalage vertical
            p1.y += wallHeight * 0.25f;
            p2.y += wallHeight * 0.25f;

            // Direction du segment
            Vector3 dir = (p2 - p1).normalized;

            // Normale perpendiculaire dans le plan XZ
            Vector3 outward = new Vector3(-dir.z, 0f, dir.x).normalized;

            // On détermine si cette normale pointe vers l’extérieur
            // en comparant avec le barycentre du polygone
            Vector3 center = PolygonCenter(loop);
            Vector3 mid = (p1 + p2) * 0.5f;

            Vector3 toCenter = (center - mid);
            toCenter.y = 0f;

            // Si la normale pointe vers l'intérieur, on l'inverse
            if (Vector3.Dot(outward, toCenter) > 0f)
                outward = -outward;

            // Application de l’offset extérieur
            p1 += outward * wallOffset;
            p2 += outward * wallOffset;

            // Étirement du mur pour compenser l’écartement
            p1 -= dir * wallOffset;
            p2 += dir * wallOffset;

            // Nouvelle longueur après offset + étirement
            float length = Vector3.Distance(p1, p2);

            // Position finale du mur
            Vector3 finalMid = (p1 + p2) * 0.5f;

            GameObject wall = new GameObject("WallSegment");
            wall.transform.SetParent(wallsRoot);
            wall.layer = wallLayer;

            wall.transform.position = finalMid;
            wall.transform.LookAt(p2);

            var col = wall.AddComponent<BoxCollider>();
            col.size = new Vector3(wallThickness, wallHeight, length);
        }
    }

    private void ClearWalls()
    {
        if (wallsRoot == null)
            return;

        var toDelete = new List<GameObject>();
        foreach (Transform child in wallsRoot)
            toDelete.Add(child.gameObject);

        foreach (var go in toDelete)
            DestroyImmediate(go);
    }

    private Vector3 PolygonCenter(List<Vector3> pts)
    {
        Vector3 sum = Vector3.zero;
        foreach (var p in pts)
            sum += p;

        return sum / pts.Count;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || outerLoop == null)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < outerLoop.Count; i++)
            Gizmos.DrawLine(outerLoop[i], outerLoop[(i + 1) % outerLoop.Count]);
    }
}