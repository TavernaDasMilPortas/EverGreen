using UnityEngine;
using System.Collections.Generic;

public class GridObjectInstantiator : MonoBehaviour
{
    public static GridObjectInstantiator Instance { get; private set; }
    [Header("Referência da personagem")]
    public Transform playerTransform;

    [Header("Árvore mais próxima")]
    public GameObject closestTree;

    [Header("Prefabs")]
    public GameObject[] objectPrefabs; // índice 0 = prefab para id 1, índice 1 = para id 2, etc.
    public GameObject midpointPrefab;
    public GameObject connectionPrefab; // Prefab visual de ligação

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

    // Novo: dicionário para armazenar midpoints, a chave é a posição no grid do midpoint
    Dictionary<Vector2Int, GameObject> midpoints = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InstantiateMap();

        // Achar posição da plataforma 1
        Vector2Int platform1Coords = FindOriginCoords(1);
        if (platform1Coords == new Vector2Int(-1, -1))
        {
            Debug.LogError("Plataforma 1 não encontrada no mapa.");
            return;
        }

        Vector3 platform1Pos = spawnedObjects[platform1Coords.x, platform1Coords.y].transform.position;

        // Direção frontal (horizontal ou vertical) — ignorar diagonais
        Vector3 forward = playerTransform.forward;
        Vector2Int direction = Vector2Int.zero;

        // Determinar a direção dominante (mais próxima do eixo principal)
        if (Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
            direction = (forward.x > 0) ? Vector2Int.right : Vector2Int.left;
        else
            direction = (forward.z > 0) ? Vector2Int.up : Vector2Int.down;

        // Posição destino
        Vector2Int targetCoords = platform1Coords + direction;

        // Verifica se está dentro do mapa
        if (targetCoords.x >= 0 && targetCoords.x < map.GetLength(0) &&
            targetCoords.y >= 0 && targetCoords.y < map.GetLength(1))
        {
            if (map[targetCoords.x, targetCoords.y] == 2) // Plataforma destino válida
            {
                Vector3 platform2Pos = spawnedObjects[targetCoords.x, targetCoords.y].transform.position;
                Vector3 midpoint = (platform1Pos + platform2Pos) / 2f + new Vector3(0, 5f, 0);

                Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
                GameObject bridge = Instantiate(connectionPrefab, midpoint, rotation, transform);
                bridge.name = $"Bridge_{platform1Coords}→{targetCoords}";
            }
            else
            {
                Debug.LogWarning("Plataforma à frente não é uma plataforma válida (valor diferente de 2).");
            }
        }
        else
        {
            Debug.LogWarning("Coordenada fora do mapa. Não é possível criar ponte.");
        }
    }

    void Update()
    {
        UpdateClosestTree();
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
        spawnedObjects = new GameObject[rows, cols];
        midpoints.Clear();

        Vector2Int originCoords = FindOriginCoords(1);
        if (originCoords == new Vector2Int(-1, -1))
        {
            Debug.LogError("Nenhuma posição com valor 1 encontrada no mapa!");
            return;
        }

        // Posição da origem do mapa (em relação à jogadora)
        Vector3 originWorldPos = playerTransform.position + new Vector3(0, verticalOffset, 0);

        // Rota o layout com base na frente da personagem
        Vector3 forward = playerTransform.forward;
        Vector3 right = playerTransform.right;

        // Instanciar plataformas
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int id = map[row, col];

                if (id > 0 && id - 1 < objectPrefabs.Length && objectPrefabs[id - 1] != null)
                {
                    int deltaRow = row - originCoords.x;
                    int deltaCol = col - originCoords.y;

                    // Cálculo da posição baseado na direção da personagem
                    Vector3 offset = right * (deltaCol * spacing) + forward * (deltaRow * spacing);
                    Vector3 finalPosition = originWorldPos + offset;

                    GameObject obj = Instantiate(objectPrefabs[id - 1], finalPosition, Quaternion.identity, transform);
                    obj.name = $"Object_{id}_({row},{col})";
                    spawnedObjects[row, col] = obj;
                }
            }
        }

        // Criar midpoints entre plataformas vizinhas (horizontal e vertical)
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (spawnedObjects[row, col] == null)
                    continue;

                // Checar vizinho à direita (col + 1)
                if (col + 1 < cols && spawnedObjects[row, col + 1] != null)
                {
                    Vector3 posA = spawnedObjects[row, col].transform.position;
                    Vector3 posB = spawnedObjects[row, col + 1].transform.position;

                    Vector3Int midpointGridPos = new Vector3Int(row, col, 0) + new Vector3Int(0, 1, 0); // posição "entre" colunas na mesma linha
                    Vector2Int midpointKey = new Vector2Int(row, col * 2 + 1); // Para armazenar posição única entre tiles

                    // Criar midpoint e armazenar
                    GameObject mid = CreateMidpoint(posA, posB);
                    mid.name = $"Midpoint_H_{row}_{col}_{col + 1}";
                    midpoints[midpointKey] = mid;
                }

                // Checar vizinho abaixo (row + 1)
                if (row + 1 < rows && spawnedObjects[row + 1, col] != null)
                {
                    Vector3 posA = spawnedObjects[row, col].transform.position;
                    Vector3 posB = spawnedObjects[row + 1, col].transform.position;

                    Vector3Int midpointGridPos = new Vector3Int(row, col, 0) + new Vector3Int(1, 0, 0); // posição "entre" linhas na mesma coluna
                    Vector2Int midpointKey = new Vector2Int(row * 2 + 1, col); // Para armazenar posição única entre tiles

                    GameObject mid = CreateMidpoint(posA, posB);
                    mid.name = $"Midpoint_V_{row}_{row + 1}_{col}";
                    midpoints[midpointKey] = mid;
                }
            }
        }
    }

    // Agora retorna o GameObject criado
    GameObject CreateMidpoint(Vector3 a, Vector3 b)
    {
        Vector3 midpoint = (a + b) / 2f;
        GameObject mid = Instantiate(midpointPrefab, midpoint, Quaternion.identity, transform);
        return mid;
    }

    public bool CanFallTree(Vector3 playerPosition, Vector3 treePosition)
    {
        if (spawnedObjects == null || midpoints == null)
        {
            Debug.LogWarning("spawnedObjects ou midpoints não inicializados.");
            return false;
        }

        // Obter direção normalizada entre árvore e jogadora (apenas no plano XZ)
        Vector3 toPlayer = (playerPosition - treePosition);
        toPlayer.y = 0; // Desconsidera altura
        Vector3 direction = toPlayer.normalized;

        // Determina a direção cardinal mais próxima
        Vector2Int gridDirection = Vector2Int.zero;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            gridDirection = direction.x > 0 ? Vector2Int.left : Vector2Int.right;
        else
            gridDirection = direction.z > 0 ? Vector2Int.down : Vector2Int.up;

        // Converter posição da árvore para coordenadas no grid
        Vector2Int treeGridPos = WorldToGrid(treePosition);

        // Posição alvo (direção oposta à jogadora)
        Vector2Int targetGridPos = treeGridPos + gridDirection;

        // Checar se está dentro do grid
        if (targetGridPos.x < 0 || targetGridPos.y < 0 ||
            targetGridPos.x >= spawnedObjects.GetLength(0) ||
            targetGridPos.y >= spawnedObjects.GetLength(1))
            return false;

        // Aqui verifica se midpoint existe na posição correspondente
        // Considerando a lógica usada para as chaves do dicionário midpoints:
        // Midpoints ficam entre tiles, então podemos mapear a posição alvo para a chave do midpoint
        // Para simplificar: o midpoint para conexão horizontal fica em col*2+1, vertical em row*2+1
        Vector2Int midpointKeyHorizontal = new Vector2Int(targetGridPos.x, targetGridPos.y * 2 - 1);
        Vector2Int midpointKeyVertical = new Vector2Int(targetGridPos.x * 2 - 1, targetGridPos.y);

        bool hasMidpoint = midpoints.ContainsKey(midpointKeyHorizontal) || midpoints.ContainsKey(midpointKeyVertical);

        return hasMidpoint;
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - (playerTransform.position + new Vector3(0, verticalOffset, 0));
        Vector3 right = playerTransform.right;
        Vector3 forward = playerTransform.forward;

        int col = Mathf.RoundToInt(Vector3.Dot(localPos, right) / spacing);
        int row = Mathf.RoundToInt(Vector3.Dot(localPos, forward) / spacing);

        // Converte para coordenadas relativas ao centro/origem
        Vector2Int originCoords = FindOriginCoords(1);
        return new Vector2Int(originCoords.x + row, originCoords.y + col);
    }

    public void SpawnBridge(Vector3 playerPosition, Vector3 treePosition)
    {
        if (spawnedObjects == null)
        {
            Debug.LogWarning("spawnedObjects não inicializado.");
            return;
        }

        // Direção da jogadora relativa à árvore, no espaço LOCAL da jogadora
        Vector3 localDir = playerTransform.InverseTransformDirection((playerPosition - treePosition).normalized);
        Vector2Int gridDirection = Vector2Int.zero;

        // Decide direção de acordo com os eixos locais
        if (Mathf.Abs(localDir.x) > Mathf.Abs(localDir.z))
            gridDirection = localDir.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            gridDirection = localDir.z > 0 ? Vector2Int.up : Vector2Int.down;

        Vector2Int treeGridPos = WorldToGrid(treePosition);
        Vector2Int targetGridPos = treeGridPos + gridDirection;

        int rows = spawnedObjects.GetLength(0);
        int cols = spawnedObjects.GetLength(1);

        if (targetGridPos.x < 0 || targetGridPos.y < 0 || targetGridPos.x >= rows || targetGridPos.y >= cols)
        {
            Debug.LogWarning("Posição alvo fora do mapa - não será instanciada ponte.");
            return;
        }

        GameObject startPlatform = spawnedObjects[treeGridPos.x, treeGridPos.y];
        GameObject targetPlatform = spawnedObjects[targetGridPos.x, targetGridPos.y];

        if (startPlatform == null || targetPlatform == null)
        {
            Debug.LogWarning("Plataforma inicial ou alvo inexistente - não será instanciada ponte.");
            return;
        }

        // Verifica se midpoint existe no dicionário (usando mesma lógica das chaves)
        Vector2Int midpointKeyH = new Vector2Int(treeGridPos.x, Mathf.Min(treeGridPos.y, targetGridPos.y) * 2 + 1);
        Vector2Int midpointKeyV = new Vector2Int(Mathf.Min(treeGridPos.x, targetGridPos.x) * 2 + 1, treeGridPos.y);

        GameObject midpointObj = null;

        if (midpoints.TryGetValue(midpointKeyH, out midpointObj) || midpoints.TryGetValue(midpointKeyV, out midpointObj))
        {
            // Usar o midpoint existente
            Vector3 midpointPos = midpointObj.transform.position;
            midpointPos.y += 0f; // se quiser ajustar verticalmente, faça aqui

            // Rotação da ponte alinhada entre start e target
            Vector3 bridgeDir = (targetPlatform.transform.position - startPlatform.transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(bridgeDir) * Quaternion.Euler(-90f, 0f, 0f);

            GameObject bridge = Instantiate(connectionPrefab, midpointPos, rotation, transform);
            bridge.name = $"Bridge_{treeGridPos.x}_{treeGridPos.y}_to_{targetGridPos.x}_{targetGridPos.y}";

            Debug.Log($"Ponte instanciada usando midpoint existente entre ({treeGridPos.x},{treeGridPos.y}) e ({targetGridPos.x},{targetGridPos.y})");
        }
        else
        {
            // Se não existe midpoint, instanciar ponte normalmente no midpoint calculado
            Vector3 midpoint = (startPlatform.transform.position + targetPlatform.transform.position) / 2f;
            midpoint.y += 5f;

            Vector3 bridgeDir = (targetPlatform.transform.position - startPlatform.transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(bridgeDir) * Quaternion.Euler(-90f, 0f, 0f);

            GameObject bridge = Instantiate(connectionPrefab, midpoint, rotation, transform);
            bridge.name = $"Bridge_{treeGridPos.x}_{treeGridPos.y}_to_{targetGridPos.x}_{targetGridPos.y}";

            Debug.LogWarning($"Ponte instanciada sem midpoint pré-existente entre ({treeGridPos.x},{treeGridPos.y}) e ({targetGridPos.x},{targetGridPos.y})");
        }
    }

    void UpdateClosestTree()
    {
        GameObject[] allTrees = GameObject.FindGameObjectsWithTag("Tree");
        Debug.Log("Árvores encontradas: " + allTrees.Length);

        if (allTrees.Length == 0 || playerTransform == null)
        {
            closestTree = null;
            Debug.LogWarning("Nenhuma árvore encontrada ou playerTransform está nulo.");
            return;
        }

        float closestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject tree in allTrees)
        {
            float distance = Vector3.Distance(playerTransform.position, tree.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = tree;
            }
        }

        closestTree = nearest;
        Debug.Log("Árvore mais próxima atualizada: " + closestTree?.name);
    }

    void OnDrawGizmos()
    {
        if (playerTransform == null || closestTree == null || spawnedObjects == null)
            return;

        if (!CanFallTree(playerTransform.position, closestTree.transform.position))
            return;

        Vector2Int treeGridPos = WorldToGrid(closestTree.transform.position);

        // Direção entre a árvore e o jogador (no plano XZ)
        Vector3 toPlayer = playerTransform.position - closestTree.transform.position;
        toPlayer.y = 0f;
        Vector3 direction = toPlayer.normalized;

        Vector2Int gridDirection;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            gridDirection = direction.x > 0 ? Vector2Int.left : Vector2Int.right;
        else
            gridDirection = direction.z > 0 ? Vector2Int.down : Vector2Int.up;

        Vector2Int targetGridPos = treeGridPos + gridDirection;

        // Verifica limites do grid
        if (targetGridPos.x < 0 || targetGridPos.y < 0 ||
            targetGridPos.x >= spawnedObjects.GetLength(0) ||
            targetGridPos.y >= spawnedObjects.GetLength(1))
            return;

        GameObject startPlatform = spawnedObjects[treeGridPos.x, treeGridPos.y];
        GameObject targetPlatform = spawnedObjects[targetGridPos.x, targetGridPos.y];

        if (startPlatform == null || targetPlatform == null)
            return;

        Vector3 start = startPlatform.transform.position;
        Vector3 end = targetPlatform.transform.position;
        Vector3 midpoint = (start + end) / 2f + Vector3.up * 5f;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(midpoint, 0.3f);
        Gizmos.DrawLine(start, end);
    }
    Vector2Int FindOriginCoords(int id)
    {
        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int col = 0; col < map.GetLength(1); col++)
            {
                if (map[row, col] == id)
                    return new Vector2Int(row, col);
            }
        }

        return new Vector2Int(-1, -1); // Retorna posição inválida se não encontrar
    }
}
