using UnityEngine;
[RequireComponent(typeof(Light))]
public class FloatingSpotLight : MonoBehaviour
{ 
    [Header("Movimento vertical")]
    public float floatAmplitude = 0.5f;   // Altura do movimento
    public float floatSpeed = 1f;         // Velocidade do movimento

    [Header("Rotação")]
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 30f); // Graus por segundo

    [Header("Luz")]
    public Color lightColor = Color.white;
    public float lightIntensity = 2f;
    public float lightRange = 5f;

    private Vector3 startPosition;
    private Light objLight;

    void Start()
    {
        startPosition = transform.position;

        objLight = GetComponent<Light>();
        if (objLight == null)
        {
            objLight = gameObject.AddComponent<Light>();
        }

        objLight.color = lightColor;
        objLight.intensity = lightIntensity;
        objLight.range = lightRange;
        objLight.type = LightType.Point;
    }

    void Update()
    {
        // Movimento flutuante (senóide)
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotação
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

