using UnityEngine;
using System.Collections.Generic;

public class OutlineManager : MonoBehaviour
{
    public static OutlineManager Instance { get; private set; }

    private List<Outline> allOutlines = new List<Outline>();
    private Outline currentActiveOutline = null;

    [Header("Configurações globais de outline")]
    public float defaultWidth = 10f;
    public Color highlightColor = Color.red;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        FindAllOutlinesInScene();
        DisableAllOutlines(); // desliga todos inicialmente
    }

    public void FindAllOutlinesInScene()
    {
        allOutlines.Clear();
        Outline[] found = FindObjectsOfType<Outline>(true);

        foreach (Outline outline in found)
        {
            // Garante que o mesh é legível
            TryMakeMeshReadable(outline);
            allOutlines.Add(outline);
        }

        Debug.Log($"[OutlineManager] Encontrados {allOutlines.Count} objetos com Outline.");
    }

    private void TryMakeMeshReadable(Outline outline)
    {
        MeshFilter mf = outline.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null && !mf.sharedMesh.isReadable)
        {
            Mesh newMesh = Instantiate(mf.sharedMesh);
            mf.mesh = newMesh; // Isso torna a cópia legível
        }

        // Se for um SkinnedMeshRenderer
        SkinnedMeshRenderer smr = outline.GetComponent<SkinnedMeshRenderer>();
        if (smr != null && smr.sharedMesh != null && !smr.sharedMesh.isReadable)
        {
            Mesh newMesh = Instantiate(smr.sharedMesh);
            smr.sharedMesh = newMesh;
        }
    }

    public void Highlight(IInteractable target)
    {
        Outline newOutline = (target as Component)?.GetComponent<Outline>();

        if (currentActiveOutline == newOutline) return;

        if (currentActiveOutline != null)
        {
            currentActiveOutline.enabled = false;
            currentActiveOutline = null;
        }

        currentActiveOutline = newOutline;

        if (currentActiveOutline != null)
        {
            currentActiveOutline.enabled = true;
            currentActiveOutline.OutlineMode = Outline.Mode.OutlineAll;
            currentActiveOutline.OutlineColor = highlightColor;
            currentActiveOutline.OutlineWidth = defaultWidth;
        }
    }

    public void DisableAllOutlines()
    {
        foreach (var outline in allOutlines)
        {
            if (outline != null)
                outline.enabled = false;
        }

        currentActiveOutline = null;
    }

    public void SetColorAllOutlines(Color newColor)
    {
        foreach (var outline in allOutlines)
        {
            if (outline != null)
                outline.OutlineColor = newColor;
        }
    }

    public void SetWidthAllOutlines(float width)
    {
        foreach (var outline in allOutlines)
        {
            if (outline != null)
                outline.OutlineWidth = width;
        }
    }
}
