using UnityEngine;
using System.Collections.Generic;

public class OutlineManager : MonoBehaviour
{
    public static OutlineManager Instance { get; private set; }

    private List<Outline> allOutlines = new List<Outline>();

    [Header("Configuração do pulso (global)")]
    public bool pulseEnabled = false;
    private float minWidth = 0f;
    public float maxWidth = 6f;
    public float pulseSpeed = 2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void Start()
    {
        // Inicializa o OutlineManager e encontra todos os outlines na cena
        FindAllOutlinesInScene();
        SetWidthAllOutlines(minWidth); // Define a largura mínima inicialmente
    }
    private void Update()
    {
 
    }

    public void FindAllOutlinesInScene()
    {
        allOutlines.Clear();
        Outline[] found = FindObjectsOfType<Outline>(true); // true: inclui inativos
        allOutlines.AddRange(found);
        Debug.Log($"[OutlineManager] Encontrados {allOutlines.Count} objetos com Outline.");
    }

    public void EnableAllOutlines()
    {
        foreach (var outline in allOutlines)
        {
            if (outline != null)
                outline.enabled = true;
        }
    }

    public void DisableAllOutlines()
    {
        foreach (var outline in allOutlines)
        {
            if (outline != null)
                outline.enabled = false;
        }
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

    public void TogglePulse(bool enable)
    {
        pulseEnabled = enable;
    }
}
