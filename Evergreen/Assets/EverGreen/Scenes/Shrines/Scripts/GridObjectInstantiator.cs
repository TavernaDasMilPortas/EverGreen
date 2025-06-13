using UnityEngine;

public class GridObjectInstantiator : MonoBehaviour
{
    [Header("Referência da personagem")]
    public Transform playerTransform;

    [Header("Prefabs")]
    public GameObject[] objectPrefabs; // índice 0 = prefab para id 1, índice 1 = para id 2, etc.
    public GameObject midpointPrefab;

    [Header("Espaçamento")]
    public float spacing = 2f;

    [Header("Offset vertical da origem")]
    public float verticalOffset = -2f;

    [Header("Mapa (linhas x colunas)")]
    public int[,] map = new int[,] {
        {0, 1, 0},
        {2, 2, 2},
        {0, 2, 0}
    };

    void Start()
    {
        InstantiateMap();
    }

    void InstantiateMap()
    {
        if (playerTransform == null)
        {
            Debug.LogError("PlayerTransform não está atribuído!");
            return;
        }

        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        GameObject[,] spawnedObjects = new GameObject[rows, cols];

        // Procurar a posição do número 1 no mapa (valor que representa a origem)
        Vector2Int originCoords = FindOriginCoords(1);
        if (originCoords == new Vector2Int(-1, -1))
        {
            Debug.LogError("Nenhuma posição com valor 1 encontrada no mapa!");
            return;
        }

        // Origem baseada na posição da personagem com offset
        Vector3 originWorldPos = playerTransform.position + new Vector3(0, verticalOffset, 0);

        // Instanciar objetos conforme o mapa
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int id = map[row, col];

                if (id > 0 && id - 1 < objectPrefabs.Length && objectPrefabs[id - 1] != null)
                {
                    // Posição relativa ao centro (origem lógica do mapa)
                    int deltaRow = row - originCoords.x;
                    int deltaCol = col - originCoords.y;

                    Vector3 position = originWorldPos + new Vector3(deltaCol * spacing, 0, -deltaRow * spacing);
                    GameObject obj = Instantiate(objectPrefabs[id - 1], position, Quaternion.identity, transform);
                    obj.name = $"Object_{id}_({row},{col})";
                    spawnedObjects[row, col] = obj;
                }
            }
        }

        // Criar pontos médios
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject current = spawnedObjects[row, col];
                if (current == null) continue;

                // Horizontal
                if (col + 1 < cols && spawnedObjects[row, col + 1] != null)
                    CreateMidpoint(current.transform.position, spawnedObjects[row, col + 1].transform.position);

                // Vertical
                if (row + 1 < rows && spawnedObjects[row + 1, col] != null)
                    CreateMidpoint(current.transform.position, spawnedObjects[row + 1, col].transform.position);
            }
        }
    }

    Vector2Int FindOriginCoords(int originValue)
    {
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                if (map[row, col] == originValue)
                    return new Vector2Int(row, col);
            }
        }
        return new Vector2Int(-1, -1); // não encontrado
    }

    void CreateMidpoint(Vector3 a, Vector3 b)
    {
        Vector3 midpoint = (a + b) / 2f;
        GameObject mid = Instantiate(midpointPrefab, midpoint, Quaternion.identity, transform);
        mid.name = "Midpoint";
    }
}
