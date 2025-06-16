using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mapas/Mapa Fase")]
public class MapData : ScriptableObject
{
    [Header("Estrutura do Mapa")]
    public List<MapRow> mapRows = new List<MapRow>();

    [Header("Pontes pré-definidas")]
    public List<MidpointConnection> bridgeConnections = new List<MidpointConnection>();

    public int maxInteractions;

    public float initialWaterY;
    public float waterLevelOffset;

    public float initialRainRate;

    public Color initialLightColor;
    public Color targetLightColor;
    public int[,] To2DArray()
    {
        int rows = mapRows.Count;
        if (rows == 0 || mapRows[0].row == null) return new int[0, 0];

        int cols = mapRows[0].row.Length;
        int[,] result = new int[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[r, c] = mapRows[r].row[c];
            }
        }

        return result;
    }

    public IEnumerable<(Vector2Int from, Vector2Int to)> GetBridgeConnections()
    {
        foreach (var conn in bridgeConnections)
        {
            yield return (conn.from, conn.to);
        }
    }
}

[System.Serializable]
public class MapRow
{
    public int[] row;
}

[System.Serializable]
public class MidpointConnection
{
    public Vector2Int from;
    public Vector2Int to;
}
