using UnityEngine;

public class OutlineURPEffect : MonoBehaviour
{
    [Tooltip("Material que usa o shader 'Custom/Outline Fill'")]
    public Material outlineMaterial;

    private Renderer rend;
    private Material[] originalMaterials;
    private bool isOutlined = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterials = rend.sharedMaterials;
        }
    }

    public void EnableOutline()
    {
        if (rend == null || outlineMaterial == null || isOutlined)
            return;

        var newMats = new Material[originalMaterials.Length + 1];
        originalMaterials.CopyTo(newMats, 0);
        newMats[originalMaterials.Length] = outlineMaterial;
        rend.materials = newMats;

        isOutlined = true;
    }

    public void DisableOutline()
    {
        if (rend == null || !isOutlined)
            return;

        rend.materials = originalMaterials;
        isOutlined = false;
    }
}
