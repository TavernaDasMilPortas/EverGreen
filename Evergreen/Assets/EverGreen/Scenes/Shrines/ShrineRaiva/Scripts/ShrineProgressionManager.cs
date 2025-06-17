using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShrineProgressionManager : MonoBehaviour
{
    public static ShrineProgressionManager Instance { get; private set; }

    [Header("Progresso")]
    private int currentInteractions = 0;
    private int maxInteractions;
    private int currentMapIndex = 0;

    [Header("Níveis de Água e Chuva")]
    private float initialWaterY;
    private float finalWaterY;
    private float waterLevelOffset;
    private float levelPerInteraction;

    private float initialRainRate;
    private float finalRainRate;

    [Header("Cores de Luz")]
     private Color initialLightColor;
     private Color targetLightColor;

    [Header("Referências")]
    public Light mainLight;
    public Transform waterPlane;
    public ParticleSystem rainSystem;
    public Transform player;

    [Header("Transições")]
    public float waterLevelForReset = 2f;
    public float waterTransitionDuration = 2f;

    [Header("Dados do Mapa")]
    public List<MapData> mapDataList = new List<MapData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        currentMapIndex = 0;
    }

    private void Start()
    {
        ResetProgression();
        ApplyData(mapDataList[currentMapIndex]);
    }


    public void ApplyDataByIndex(int index)
    {
        if (mapDataList == null || index < 0 || index >= mapDataList.Count)
        {
            Debug.LogError($"Índice inválido ({index}) para MapData. Lista tem {mapDataList?.Count ?? 0} elementos.");
            return;
        }

        MapData data = mapDataList[index];
        ApplyData(data);
    }

    public void ApplyData(MapData data)
    {
        if (data == null)
        {
            Debug.LogWarning("MapData é nulo.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player não atribuído no ShrineProgressionManager.");
            return;
        }

        maxInteractions = Mathf.Max(1, data.maxInteractions) + 1; // evita divisão por zero
        initialWaterY = data.initialWaterY;
        waterLevelOffset = data.waterLevelOffset;
        initialRainRate = data.initialRainRate;
        initialLightColor = data.initialLightColor;
        targetLightColor = data.targetLightColor;

        finalWaterY = player.position.y + waterLevelOffset;
        levelPerInteraction = (finalWaterY - initialWaterY) / maxInteractions;
        finalRainRate = initialRainRate * maxInteractions;

        Debug.Log($"[ShrineProgressionManager] Dados aplicados (Fase {currentMapIndex}):\n" +
                  $"- maxInteractions: {maxInteractions}\n" +
                  $"- initialWaterY: {initialWaterY}\n" +
                  $"- finalWaterY: {finalWaterY}\n" +
                  $"- levelPerInteraction: {levelPerInteraction}\n" +
                  $"- initialRainRate: {initialRainRate}\n" +
                  $"- finalRainRate: {finalRainRate}\n" +
                  $"- initialLightColor: {initialLightColor}\n" +
                  $"- targetLightColor: {targetLightColor}");
    }

    public void ResetProgression()
    {
        currentInteractions = 0;

        if (mainLight != null)
            mainLight.color = initialLightColor;

        if (waterPlane != null)
            waterPlane.position = new Vector3(waterPlane.position.x, initialWaterY, waterPlane.position.z);

        var emission = rainSystem.emission;
        emission.rateOverTime = initialRainRate;
    }


    public void RegisterInteraction()
    {
        currentInteractions = Mathf.Clamp(currentInteractions + 1, 0, maxInteractions);
        float t = (float)currentInteractions / maxInteractions;

        if (mainLight != null)
            mainLight.color = Color.Lerp(initialLightColor, targetLightColor, t);

        if (waterPlane != null)
        {
            float targetY = Mathf.Lerp(initialWaterY, finalWaterY, t);
            StartCoroutine(SmoothMoveWater(waterPlane.position.y, targetY));
        }

        var emission = rainSystem.emission;
        emission.rateOverTime = Mathf.Lerp(initialRainRate, finalRainRate, t);
    }

    private IEnumerator SmoothMoveWater(float startY, float targetY)
    {
        float elapsed = 0f;
        Vector3 startPos = waterPlane.position;
        Vector3 endPos = new Vector3(startPos.x, targetY, startPos.z);

        while (elapsed < waterTransitionDuration)
        {
            float t = elapsed / waterTransitionDuration;
            waterPlane.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        waterPlane.position = endPos;
    }

    // Avançar para próxima fase (caso queira controlar isso manualmente)
    public void AdvancePhase()
    {
        if (currentMapIndex + 1 < mapDataList.Count)
        {
            currentMapIndex++;
            ApplyDataByIndex(currentMapIndex);
            ResetProgression();
        }
        else
        {
            Debug.Log("Última fase já foi alcançada.");
        }
    }
}
