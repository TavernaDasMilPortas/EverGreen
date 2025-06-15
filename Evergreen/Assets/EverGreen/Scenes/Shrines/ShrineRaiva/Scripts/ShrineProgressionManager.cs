using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShrineProgressionManager : MonoBehaviour
{

    [Header("Configuração")]
    public int maxInteractions = 5;
    private int currentInteractions = 0;

    [Header("Ambiente")]
    public Light mainLight;
    public Color initialLightColor = Color.white;
    public Color targetLightColor = Color.blue;
    public float lightLerpSpeed = 1f;

    [Header("Água")]
    public Transform waterPlane;
    public float initialWaterY = 26f;
    public float finalWaterY;
    public float levelPerInteraction;

    [Header("Chuva")]
    public ParticleSystem rainSystem;
    private ParticleSystem.EmissionModule rainEmission;
    [SerializeField]private float initialRainRate = 100f;
    private float finalRainRate;

    [Header("Jogadora")]
    public Transform player;
    public float waterLevelForReset = 2f; // altura da água relativa à jogadora
    
    [Header("Transições")]
    public float waterTransitionDuration = 2f;

    void Start()
    {
        if (mainLight == null)
            mainLight = GameObject.FindGameObjectWithTag("MainLight")?.GetComponent<Light>();

        var emission = rainSystem.emission;
        emission.rateOverTime = initialRainRate;

        if (waterPlane != null)
            waterPlane.position = new Vector3(waterPlane.position.x, initialWaterY, waterPlane.position.z);

        finalWaterY = player.position.y + 2f;
        levelPerInteraction = (finalWaterY - initialWaterY) / maxInteractions;
        finalRainRate = initialRainRate * maxInteractions;
    }

    public void RegisterInteraction()
    {
        currentInteractions++;
        currentInteractions = Mathf.Clamp(currentInteractions, 0, maxInteractions);

        float t = (float)currentInteractions / maxInteractions;

        // Luz ambiente
        if (mainLight != null)
            mainLight.color = Color.Lerp(initialLightColor, targetLightColor, t);

        // Subida da água
        if (waterPlane != null)
        {
            float targetY = Mathf.Lerp(initialWaterY, finalWaterY, t);
            StartCoroutine(SmoothMoveWater(waterPlane.position.y, targetY));
        }

        // Intensidade da chuva
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

}
