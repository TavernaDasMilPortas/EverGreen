using UnityEngine;
using System.Collections.Generic;

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

public class GridObjectInstantiator : MonoBehaviour
{
    public static GridObjectInstantiator Instance { get; private set; }

    [Header("Referência da personagem")]
    public Transform playerTransform;

    [Header("Árvore mais próxima")]
    public GameObject closestTree;

    [Header("Prefabs")]
    public GameObject[] objectPrefabs;
    public GameObject midpointPrefab;
    public GameObject connectionPrefab;

    [Header("Espaçamento")]
    public float spacing = 2f;

    [Header("Offset vertical da origem")]
    public float verticalOffset = -2f;

    [Header("Mapa (linhas x colunas)")]
    public int[,] map = new int[,] {
        {0, 2, 1, 2, 0},
        {2, 2, 2, 2, 2},
        {0, 2, 2, 2, 0}
    };

    GameObject[,] spawnedObjects;
    List<PlatformMidpoint> midpoints = new List<PlatformMidpoint>();

    void Awake() => Instance = this;

    void Start() => InstantiateMap();

    void Update() => UpdateClosestTree();

    void InstantiateMap()
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        spawnedObjects = new GameObject[rows, cols];
        midpoints.Clear();

        Vector2Int originCoords = FindOriginCoords(1);
        if (originCoords == new Vector2Int(-1, -1)) return;

        Vector3 originWorldPos = playerTransform.position + new Vector3(0, verticalOffset, 0);
        Vector3 forward = playerTransform.forward;
        Vector3 right = playerTransform.right;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int id = map[row, col];
                if (id > 0 && id - 1 < objectPrefabs.Length && objectPrefabs[id - 1] != null)
                {
                    int deltaRow = row - originCoords.x;
                    int deltaCol = col - originCoords.y;
                    Vector3 offset = right * (deltaCol * spacing) + forward * (deltaRow * spacing);
                    Vector3 finalPosition = originWorldPos + offset;

                    GameObject obj = Instantiate(objectPrefabs[id - 1], finalPosition, Quaternion.identity, transform);
                    obj.name = $"Object_{id}_({row},{col})";
                    spawnedObjects[row, col] = obj;
                }
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (spawnedObjects[row, col] == null) continue;

                Vector2Int current = new Vector2Int(row, col);

                Vector2Int[] directions = new[] {
                    new Vector2Int(0, 1), new Vector2Int(1, 0)
                };

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = current + dir;
                    if (neighbor.x < rows && neighbor.y < cols && spawnedObjects[neighbor.x, neighbor.y] != null)
                    {
                        Vector3 posA = spawnedObjects[current.x, current.y].transform.position;
                        Vector3 posB = spawnedObjects[neighbor.x, neighbor.y].transform.position;
                        Vector3 mid = (posA + posB) / 2f;

                        GameObject midObj = Instantiate(midpointPrefab, mid, Quaternion.identity, transform);
                        midObj.name = $"Midpoint_{current.x}_{current.y}_to_{neighbor.x}_{neighbor.y}";

                        midpoints.Add(new PlatformMidpoint
                        {
                            from = current,
                            to = neighbor,
                            midpointObj = midObj
                        });
                    }
                }
            }
        }
    }

    Vector2Int FindOriginCoords(int id)
    {
        for (int row = 0; row < map.GetLength(0); row++)
            for (int col = 0; col < map.GetLength(1); col++)
                if (map[row, col] == id)
                    return new Vector2Int(row, col);
        return new Vector2Int(-1, -1);
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - (playerTransform.position + new Vector3(0, verticalOffset, 0));
        Vector3 right = playerTransform.right;
        Vector3 forward = playerTransform.forward;
        int col = Mathf.RoundToInt(Vector3.Dot(localPos, right) / spacing);
        int row = Mathf.RoundToInt(Vector3.Dot(localPos, forward) / spacing);
        Vector2Int originCoords = FindOriginCoords(1);
        return new Vector2Int(originCoords.x + row, originCoords.y + col);
    }

    public bool CanFallTree(Vector3 playerPos, Vector3 treePos)
    {
        Vector3 dir = (playerPos - treePos).normalized;
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
        Vector3 dir = (playerPos - treePos).normalized;
        dir.y = 0;

        Vector2Int direction = Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ?
            (dir.x > 0 ? Vector2Int.left : Vector2Int.right) :
            (dir.z > 0 ? Vector2Int.down : Vector2Int.up);

        Vector2Int treeGrid = WorldToGrid(treePos);
        Vector2Int targetGrid = treeGrid + direction;

        if (!IsInsideGrid(treeGrid) || !IsInsideGrid(targetGrid)) return;

        GameObject start = spawnedObjects[treeGrid.x, treeGrid.y];
        GameObject end = spawnedObjects[targetGrid.x, targetGrid.y];

        if (start == null || end == null) return;

        PlatformMidpoint foundMidpoint = midpoints.Find(m => m.Matches(treeGrid, targetGrid));

        Vector3 midpointPos = foundMidpoint != null ? foundMidpoint.midpointObj.transform.position :
            (start.transform.position + end.transform.position) / 2f + Vector3.up * 5f;

        Quaternion rot = Quaternion.LookRotation((end.transform.position - start.transform.position).normalized) *
                         Quaternion.Euler(-90f, 0f, 0f);

        GameObject bridge = Instantiate(connectionPrefab, midpointPos, rot, transform);
        bridge.name = $"Bridge_{treeGrid}_to_{targetGrid}";
    }

    bool IsInsideGrid(Vector2Int coord)
    {
        return coord.x >= 0 && coord.y >= 0 &&
               coord.x < spawnedObjects.GetLength(0) &&
               coord.y < spawnedObjects.GetLength(1);
    }

    void UpdateClosestTree()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        float minDist = float.MaxValue;
        GameObject nearest = null;
        foreach (var tree in trees)
        {
            float dist = Vector3.Distance(playerTransform.position, tree.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = tree;
            }
        }
        closestTree = nearest;
    }
}
