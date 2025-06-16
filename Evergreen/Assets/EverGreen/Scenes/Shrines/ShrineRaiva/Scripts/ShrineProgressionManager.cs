using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShrineProgressionManager : MonoBehaviour
{

    public static ShrineProgressionManager Instance {  get; private set; } 
    private int currentInteractions = 0;

    private int maxInteractions;
    private float initialWaterY;
    private float finalWaterY;
    private float initialRainRate;
    private float finalRainRate;
    private float levelPerInteraction;
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

    void Start()
    {
        ResetProgression();
        StartCoroutine(ApplyDataNextFrame());
 // AAAAAAAAAAAAAA

    }

    public void ApplyData(MapData data)
    {
        if (data == null) return;

        maxInteractions = data.maxInteractions;
        initialWaterY = data.initialWaterY;
        initialRainRate = data.initialRainRate;
        initialLightColor = data.initialLightColor;
        targetLightColor = data.targetLightColor;

        finalWaterY = player.position.y + data.waterLevelOffset;
        levelPerInteraction = (finalWaterY - initialWaterY) / maxInteractions;
        finalRainRate = initialRainRate * maxInteractions;
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
        currentInteractions++;
        currentInteractions = Mathf.Clamp(currentInteractions, 0, maxInteractions);

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
    private IEnumerator ApplyDataNextFrame()
    {
        // Aguarda até que MapGenerator.Instance esteja disponível
        while (MapGenerator.Instance == null || MapGenerator.Instance.mapDataList.Count == 0)
            yield return null;

        ApplyData(MapGenerator.Instance.mapDataList[MapGenerator.Instance.currentPhaseIndex]);
    }
}
