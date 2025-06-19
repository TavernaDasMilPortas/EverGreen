// Anexe esse script no MouseFollower
using UnityEngine;

public class MouseMarkerSpawner : MonoBehaviour
{
    void Start()
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(marker.GetComponent<Collider>());

        marker.transform.SetParent(transform);
        marker.transform.localPosition = Vector3.zero;
        marker.transform.localScale = Vector3.one * 0.2f;

        var mat = new Material(Shader.Find("Standard"));
        mat.color = Color.cyan;
        marker.GetComponent<Renderer>().material = mat;
    }
}
