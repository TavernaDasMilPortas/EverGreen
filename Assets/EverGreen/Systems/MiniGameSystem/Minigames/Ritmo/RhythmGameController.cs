using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RhythmGameController : MonoBehaviour
{
    [Header("Configurações")]
    public RhythmDifficultyData difficultyData;
    public GameObject notePrefab;
    public RectTransform noteArea;
    public TextMeshProUGUI feedbackText;

    [Header("Modo de Jogo")]
    public RhythmGameMode selectedMode;

    private IRhythmGameMode currentMode;
    private NoteSpawnerService spawnerService;
    private NoteEvaluatorService evaluatorService;

    private List<RhythmNote> activeNotes = new List<RhythmNote>();

    private void Start()
    {
        spawnerService = new NoteSpawnerService();
        evaluatorService = new NoteEvaluatorService();

        SetMode(RhythmGameModeFactory.Create(selectedMode));
    }

    private void Update()
    {
        currentMode?.UpdateMode();

        foreach (var note in activeNotes.ToArray())
        {
            note.MoveNote(Time.deltaTime);

            if (note.IsExpired())
            {
                activeNotes.Remove(note);
                Destroy(note.gameObject);
                feedbackText.text = "Miss!";
            }
        }

        // Exemplo de input (tecla pressionada)
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    currentMode?.HandleInput(key);
                    break;
                }
            }
        }
    }

    public void SetMode(IRhythmGameMode mode)
    {
        currentMode = mode;
        currentMode.Initialize(this);
    }

    public void SpawnNote(char key, Vector2 position)
    {
        var note = spawnerService.SpawnNote(notePrefab, noteArea, key, position);
        activeNotes.Add(note);
    }

    public void CheckNoteHit(KeyCode key)
    {
        foreach (var note in activeNotes)
        {
            if (note.MatchesKey(key))
            {
                var result = evaluatorService.EvaluateHit(note);
                feedbackText.text = result.ToString();
                activeNotes.Remove(note);
                Destroy(note.gameObject);
                return;
            }
        }

        feedbackText.text = "Miss!";
    }

    public List<RhythmNote> GetActiveNotes() => activeNotes;
}
