using UnityEngine;
using TMPro;

public class NoteSpawnerService
{
    public RhythmNote SpawnNote(GameObject prefab, RectTransform area, char key, Vector2 position)
    {
        GameObject noteGO = GameObject.Instantiate(prefab, area);
        noteGO.GetComponentInChildren<TextMeshProUGUI>().text = key.ToString();

        RectTransform rect = noteGO.GetComponent<RectTransform>();
        rect.anchoredPosition = position;

        return noteGO.GetComponent<RhythmNote>();
    }
}
