using UnityEngine;
using System.Collections.Generic;



public class MidpointManager : MonoBehaviour
{
    public class ClosestTreeData
    {
        public GameObject treeObject;
        public Vector2Int platformGridPos;
        public List<PlatformMidpoint> adjacentMidpoints;

        public ClosestTreeData(GameObject tree, Vector2Int gridPos, List<PlatformMidpoint> midpoints)
        {
            treeObject = tree;
            platformGridPos = gridPos;
            adjacentMidpoints = midpoints;
        }
    }
    public class PlatformMidpoint
    {
        public Vector2Int from;
        public Vector2Int to;
        public GameObject midpointObj;

        public bool Matches(Vector2Int a, Vector2Int b)
        {
            return (from == a && to == b) || (from == b && to == a);
        }
    }

    public static MidpointManager Instance { get; private set; }

    public GameObject midpointPrefab;
    public GameObject connectionPrefab;
    public Transform playerTransform;
    public float gizmoHeight = 0.2f;

    public MapGenerator mapGen;
    public List<PlatformMidpoint> midpoints = new List<PlatformMidpoint>();
    public ClosestTreeData closestTree;

    private PlatformMidpoint currentMidpoint; // NOVO

    void Awake() => Instance = this;

    void Update()
    {
        if (mapGen == null || mapGen.spawnedObjects == null)
            return;

        UpdateClosestTree();
        UpdateCurrentMidpoint();
    }
    
    void OnDrawGizmos()
    {
        if (currentMidpoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentMidpoint.midpointObj.transform.position + Vector3.up * gizmoHeight, 0.3f);
        }
    }

    void UpdateCurrentMidpoint()
    {
        currentMidpoint = null;

        if (closestTree == null || mapGen == null || playerTransform == null) return;

        Vector3 playerPos = playerTransform.position;
        Vector3 treePos = closestTree.treeObject.transform.position;

        Vector3 dir = (treePos - playerPos).normalized;
        dir.y = 0;

        Vector2Int direction = Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ?
            (dir.x > 0 ? Vector2Int.down : Vector2Int.up) :
            (dir.z > 0 ? Vector2Int.left : Vector2Int.right);

        Vector2Int from = closestTree.platformGridPos;
        Vector2Int to = from + direction;

        currentMidpoint = closestTree.adjacentMidpoints.Find(m => m.Matches(from, to));
    }


    public void RegisterMidpoint(Vector2Int from, Vector2Int to, GameObject obj)
    {
        midpoints.Add(new PlatformMidpoint { from = from, to = to, midpointObj = obj });
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float minDist = float.MaxValue;
        Vector2Int closestCell = new Vector2Int(-1, -1);

        int rows = mapGen.spawnedObjects.GetLength(0);
        int cols = mapGen.spawnedObjects.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject obj = mapGen.spawnedObjects[row, col];
                if (obj == null) continue;

                float dist = Vector3.Distance(worldPos, obj.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestCell = new Vector2Int(row, col);
                }
            }
        }

        if (closestCell == new Vector2Int(-1, -1))
        {
            Debug.LogWarning("Nenhuma plataforma encontrada próxima à posição.");
        }

        return closestCell;
    }

    void UpdateClosestTree()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        float minDist = float.MaxValue;
        GameObject nearestTree = null;

        foreach (var tree in trees)
        {
            float dist = Vector3.Distance(playerTransform.position, tree.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestTree = tree;
            }
        }

        if (nearestTree == null)
        {
            closestTree = null;
            return;
        }

        Vector2Int treeGrid = WorldToGrid(nearestTree.transform.position);
        Vector2Int[] offsets = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<PlatformMidpoint> adjacents = new List<PlatformMidpoint>();

        foreach (var offset in offsets)
        {
            Vector2Int neighbor = treeGrid + offset;
            PlatformMidpoint mp = midpoints.Find(m => m.Matches(treeGrid, neighbor));
            if (mp != null)
                adjacents.Add(mp);
        }

        closestTree = new ClosestTreeData(nearestTree, treeGrid, adjacents);
    }

    public void ClearMidpoints()
    {
        foreach (var m in midpoints)
        {
            if (m.midpointObj != null)
                Destroy(m.midpointObj);
        }
        midpoints.Clear();
    }
    public bool CanFallTree(Vector3 playerPos, Vector3 treePos)
    {
        Vector3 dir = (treePos - playerPos).normalized;
        dir.y = 0;

        Vector2Int direction = Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ?
            (dir.x > 0 ? Vector2Int.left : Vector2Int.right) :
            (dir.z > 0 ? Vector2Int.down : Vector2Int.up);

        Vector2Int treeGrid = WorldToGrid(treePos);
        Vector2Int targetGrid = treeGrid + direction;

        return midpoints.Exists(m => m.Matches(treeGrid, targetGrid));
    }

    public void SpawnBridgeFromTree(Vector3 playerPos, Vector3 treePos)
    {
        if (currentMidpoint == null)
        {
            Debug.LogWarning("Nenhum midpoint válido ao redor da plataforma atual.");
            return;
        }

        SpawnBridgeBetween(currentMidpoint.from, currentMidpoint.to);
    }

    public void SpawnBridgeBetween(Vector2Int from, Vector2Int to)
    {
        PlatformMidpoint midpoint = midpoints.Find(m => m.Matches(from, to));
        if (midpoint == null)
        {
            Debug.LogWarning($"Nenhum midpoint encontrado entre {from} e {to}.");
            return;
        }

        GameObject start = mapGen.spawnedObjects[from.x, from.y];
        GameObject end = mapGen.spawnedObjects[to.x, to.y];

        if (start == null || end == null)
        {
            Debug.LogWarning($"Plataforma nula entre {from} e {to}.");
            return;
        }

        Vector3 midpointPos = midpoint.midpointObj.transform.position;

        // Define rotação com base na direção da ponte
        Quaternion rot;
        if (from.x != to.x) // Ligação vertical (linha diferente)
            rot = Quaternion.Euler(-90f, 90f, 0f); // Apontando no eixo Z
        else // Ligação horizontal (coluna diferente)
            rot = Quaternion.Euler(-90f, 0f, 0f); // Apontando no eixo X

        GameObject bridge = Instantiate(connectionPrefab, midpointPos + new Vector3(0f, 5f,0f), rot, transform);
        bridge.name = $"Bridge_{from}_to_{to}";
    }
    
}
