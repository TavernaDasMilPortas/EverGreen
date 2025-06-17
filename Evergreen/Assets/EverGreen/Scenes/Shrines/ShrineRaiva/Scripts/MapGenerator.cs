using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    [Header("Referência da personagem")]
    public Transform playerTransform;

    [Header("Prefabs")]
    public GameObject[] objectPrefabs;
    public GameObject midpointPrefab;

    [Header("Espaçamento")]
    public float spacing = 2f;

    [Header("Offset vertical da origem")]
    public float verticalOffset = -2f;

    [Header("Start point do mapa (mundo)")]
    public Transform startPoint; // ← definido no Inspector

    public int currentPhaseIndex = 0;

    [Header("Mapa (linhas x colunas)")]
    public int[,] map;

    public GameObject[,] spawnedObjects;

    [Header("Fases pré-configuradas")]
    public List<MapData> mapDataList;

    [Header("Câmeras")]
    public Cinemachine.CinemachineVirtualCamera topDownCamera;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (mapDataList != null && mapDataList.Count > 0)
        {
            currentPhaseIndex = 0;
            GenerateMap(mapDataList[currentPhaseIndex].To2DArray());
        }
    }

    void InstantiateMap()
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        spawnedObjects = new GameObject[rows, cols];

        Vector2Int originCoords = FindOriginCoords(1); // usa plataforma de id=1
        if (originCoords == new Vector2Int(-1, -1)) return;

        Vector3 originWorldPos = startPoint.position + new Vector3(0, verticalOffset, 0);
        Vector3 forward = startPoint.forward;
        Vector3 right = startPoint.right;

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

        // Midpoints
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (map[row, col] == 0) continue;

                Vector3 currentPos = spawnedObjects[row, col]?.transform.position ?? Vector3.zero;

                Vector2Int[] directions = new Vector2Int[] {
                    Vector2Int.right,
                    Vector2Int.down
                };

                foreach (var dir in directions)
                {
                    int newRow = row + dir.x;
                    int newCol = col + dir.y;

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                    {
                        if (map[newRow, newCol] != 0 && spawnedObjects[newRow, newCol] != null)
                        {
                            Vector3 neighborPos = spawnedObjects[newRow, newCol].transform.position;
                            Vector3 midPos = (currentPos + neighborPos) / 2;

                            Vector2Int from = new Vector2Int(row, col);
                            Vector2Int to = new Vector2Int(newRow, newCol);

                            if (!MidpointManager.Instance.midpoints.Exists(m => m.Matches(from, to)))
                            {
                                GameObject midpointObj = Instantiate(midpointPrefab, midPos, Quaternion.identity, transform);
                                midpointObj.name = $"midPoint_{from.x}{from.y}_{to.x}{to.y}";
                                MidpointManager.Instance?.RegisterMidpoint(from, to, midpointObj);
                            }
                        }
                    }
                }
            }
        }

        // Mover o jogador diretamente para a plataforma de id = 1
        MovePlayerToSpawn(originCoords, originWorldPos, forward, right);
        PositionTopDownCamera();
    }

    void MovePlayerToSpawn(Vector2Int originCoords, Vector3 originWorldPos, Vector3 forward, Vector3 right)
    {
        Vector3 offset = Vector3.zero; // já está na origem
        Vector3 finalPosition = originWorldPos + offset;

        playerTransform.position = finalPosition;
        playerTransform.rotation = Quaternion.Euler(0f, -180f, 0f);
    }

    void ClearMap()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (spawnedObjects != null)
        {
            int rows = spawnedObjects.GetLength(0);
            int cols = spawnedObjects.GetLength(1);

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    if (spawnedObjects[row, col] != null)
                        Destroy(spawnedObjects[row, col]);
        }

        MidpointManager.Instance.ClearMidpoints();
    }

    public void NextPhase()
    {
        currentPhaseIndex++;
        if (currentPhaseIndex >= mapDataList.Count)
            currentPhaseIndex = 0;

        GenerateMap(mapDataList[currentPhaseIndex].To2DArray());
        ShrineProgressionManager.Instance.AdvancePhase();
    }

    public void GenerateMap(int[,] newMap)
    {
        ClearMap();
        this.map = newMap;
        InstantiateMap();

        var currentMapData = mapDataList[currentPhaseIndex];

        foreach (var (from, to) in currentMapData.GetBridgeConnections())
        {
            MidpointManager.Instance.SpawnBridgeBetween(from, to);
        }
    }

    public Vector2Int FindOriginCoords(int id)
    {
        for (int row = 0; row < map.GetLength(0); row++)
            for (int col = 0; col < map.GetLength(1); col++)
                if (map[row, col] == id)
                    return new Vector2Int(row, col);

        return new Vector2Int(-1, -1);
    }

    void PositionTopDownCamera()
    {
        if (topDownCamera == null) return;

        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        float sumX = 0f, sumZ = 0f;
        int count = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (spawnedObjects[row, col] != null)
                {
                    Vector3 pos = spawnedObjects[row, col].transform.position;
                    sumX += pos.x;
                    sumZ += pos.z;
                    count++;
                }
            }
        }

        if (count == 0) return;

        Vector3 center = new Vector3(sumX / count, 0, sumZ / count);
        Vector3 cameraPos = center + Vector3.up * 20f;

        topDownCamera.transform.position = cameraPos;
        topDownCamera.transform.rotation = Quaternion.Euler(90f, -90f, 0f);
        topDownCamera.LookAt = null; // ou mantenha null para visão ortogonal
    }

}

