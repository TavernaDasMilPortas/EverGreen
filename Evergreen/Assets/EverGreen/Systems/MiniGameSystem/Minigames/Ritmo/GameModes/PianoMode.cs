using System.Collections.Generic;
using UnityEngine;

public class PianoMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private float gameTimer;
    private float nextNoteTime;
    private List<RhythmNote> activeNotes = new List<RhythmNote>();

    public bool IsModeFinished { get; private set; } = false;

    public PianoMode(IRhythmGameController controller)
    {
        this.controller = controller;
    }

    public void Initialize(IRhythmGameController controller)
    {
        this.controller = controller;
    }

    public void StartMode()
    {
        gameTimer = controller.difficultyData.piano_gameDuration;
        nextNoteTime = Random.Range(controller.difficultyData.piano_minTimeBetweenNotes, controller.difficultyData.piano_maxTimeBetweenNotes);
    }

    public void UpdateMode()
    {
        gameTimer -= Time.deltaTime;
        controller.timerText.text = Mathf.CeilToInt(gameTimer).ToString();

        if (gameTimer <= 0)
        {
            IsModeFinished = true;
            return;
        }

        nextNoteTime -= Time.deltaTime;

        if (nextNoteTime <= 0f)
        {
            SpawnNote();
            nextNoteTime = Random.Range(controller.difficultyData.piano_minTimeBetweenNotes, controller.difficultyData.piano_maxTimeBetweenNotes);
        }

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            RhythmNote note = activeNotes[i];

            Vector2 pos = note.rectTransform.anchoredPosition;
            pos.y -= controller.difficultyData.piano_noteSpeed * Time.deltaTime;
            note.rectTransform.anchoredPosition = pos;

            if (note.IsPastHitZone())
            {
                controller.feedbackText.text = "Errou!";
                note.DestroyNote();
                activeNotes.RemoveAt(i);
            }
        }
    }

    public void HandleInput(KeyCode key)
    {
        string pressedKey = key.ToString().ToLower();
        Debug.Log($"Tecla pressionada: {pressedKey}");

        RhythmNote bestCandidate = null;
        float closestDistance = float.MaxValue;

        foreach (var note in activeNotes)
        {
            if (note.MatchesKey(key))
            {
                float distance = note.DistanceToHitArea();
                Debug.Log($"Nota {note.key} corresponde. Distância: {distance}");

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestCandidate = note;
                }
            }
        }

        if (bestCandidate != null)
        {
            Debug.Log($"Melhor candidata: {bestCandidate.key} com distância: {closestDistance}");

            string accuracy = EvaluateAccuracy(closestDistance);
            Debug.Log($"Nota {bestCandidate.key} acertada com precisão: {accuracy} (distância: {closestDistance})");

            controller.feedbackText.text = accuracy;
            activeNotes.Remove(bestCandidate);
            bestCandidate.OnHit();
        }
        else
        {
            Debug.LogWarning("Nenhuma nota correspondente encontrada para a tecla pressionada.");
            controller.feedbackText.text = "Errou!";
        }
    }

    private void SpawnNote()
    {
        char randomKey = controller.difficultyData.allowedKeys[Random.Range(0, controller.difficultyData.allowedKeys.Length)];
        Debug.Log($"Spawnando nova nota: {randomKey}");

        RectTransform areaRect = controller.noteArea.GetComponent<RectTransform>();
        float areaWidth = areaRect.rect.width;
        int keyIndex = controller.difficultyData.allowedKeys.IndexOf(randomKey);
        float slotWidth = areaWidth / controller.difficultyData.allowedKeys.Length;
        float x = -areaWidth / 2f + slotWidth * (keyIndex + 0.5f);
        Vector2 spawnPos = new Vector2(x, areaRect.rect.height / 2f + 50f); // Ajuste de 50f como offset superior
        Transform parentTransform = GameObject.Find("MiniGamePanel").transform;

        GameObject noteGO = RhythmNote.SpawnNote(
            randomKey,
            controller.hitZone,
            controller.noteButtonPrefab,
            parentTransform,
            spawnPos,
            NoteType.Piano,
            0
        );

        RhythmNote rhythmNote = noteGO.GetComponent<RhythmNote>();
        if (rhythmNote == null)
        {
            Debug.LogError("RhythmNote component não encontrado no noteButtonPrefab!");
            return;
        }

        activeNotes.Add(rhythmNote);
    }

    private string EvaluateAccuracy(float distance)
    {
        if (distance <= 10f) return "Perfeito!";
        if (distance <= 20f) return "Bom!";
        if (distance <= 30f) return "Ok!";
        if (distance <= 40f) return "Ruim!";
        return "Errou!";
    }
}
