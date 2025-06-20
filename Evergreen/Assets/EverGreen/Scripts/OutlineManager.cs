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
        allOutlines.AddRange(found);
        Debug.Log($"[OutlineManager] Encontrados {allOutlines.Count} objetos com Outline.");
    }

    public void Highlight(IInteractable target)
    {
        Outline newOutline = (target as Component)?.GetComponent<Outline>();

        // Se for o mesmo, não faz nada
        if (currentActiveOutline == newOutline) return;

        // Desativa o atual
        if (currentActiveOutline != null)
        {
            currentActiveOutline.enabled = false;
            currentActiveOutline = null;
        }

        currentActiveOutline = newOutline;

        // Ativa novo se existir
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
