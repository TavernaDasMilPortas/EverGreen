using System.Collections.Generic;
using UnityEngine;

public class PianoMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private float gameTimer;
    private float nextNoteTime;
    private List<RhythmNote> activeNotes = new List<RhythmNote>();

    public bool IsModeFinished { get; private set; } = false;

    public void Initialize(IRhythmGameController controller)
    {
        this.controller = controller;
    }
    public PianoMode(IRhythmGameController controller)
    {
        this.controller = controller;
    }

    public void StartMode()
    {
        gameTimer = controller.difficultyData.piano_gameDuration;
        ScheduleNextNote();
    }

    public void UpdateMode()
    {
        gameTimer -= Time.deltaTime;
        controller.timerText.text = Mathf.CeilToInt(gameTimer).ToString();

        if (Time.time >= nextNoteTime)
        {
            SpawnNote();
            ScheduleNextNote();
        }

        // Move todas as notas
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            RhythmNote note = activeNotes[i];
            note.MoveDown(controller.difficultyData.piano_noteSpeed);

            if (note.IsOutOfBounds())
            {
                controller.feedbackText.text = "Errou!";
                GameObject.Destroy(note.gameObject);
                activeNotes.RemoveAt(i);
            }
        }

        if (gameTimer <= 0)
        {
            IsModeFinished = true;
        }
    }

    public void HandleInput(KeyCode key)
    {
        string pressedKey = key.ToString().ToUpper();

        RhythmNote bestCandidate = null;
        float closestDistance = float.MaxValue;

        foreach (var note in activeNotes)
        {
            if (note.GetKey().ToString().ToUpper() == pressedKey)
            {
                float distance = Mathf.Abs(note.GetVerticalDistanceToHitZone());

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestCandidate = note;
                }
            }
        }

        if (bestCandidate != null)
        {
            string accuracy = EvaluateAccuracy(closestDistance);
            controller.feedbackText.text = accuracy;
            activeNotes.Remove(bestCandidate);
            GameObject.Destroy(bestCandidate.gameObject);
        }
        else
        {
            controller.feedbackText.text = "Errou!";
        }
    }

    private string EvaluateAccuracy(float distance)
    {
        if (distance <= 10f) return "Perfeito!";
        if (distance <= 20f) return "Bom!";
        if (distance <= 30f) return "Ok!";
        if (distance <= 40f) return "Ruim!";
        return "Errou!";
    }

    private void SpawnNote()
    {
        char randomKey = controller.difficultyData.allowedKeys[Random.Range(0, controller.difficultyData.allowedKeys.Length)];
        GameObject newNoteGO = GameObject.Instantiate(controller.noteButtonPrefab, controller.noteArea);

        RectTransform noteRect = newNoteGO.GetComponent<RectTransform>();
        RectTransform areaRect = controller.noteArea.GetComponent<RectTransform>();

        float areaWidth = areaRect.rect.width;

        // Definir posição horizontal por "casa" da tecla (QWER em colunas fixas)
        int keyIndex = controller.difficultyData.allowedKeys.IndexOf(randomKey);
        float slotWidth = areaWidth / controller.difficultyData.allowedKeys.Length;
        float x = -areaWidth / 2f + slotWidth * (keyIndex + 0.5f);

        // Spawn no topo
        noteRect.anchoredPosition = new Vector2(x, areaRect.rect.height / 2f + noteRect.rect.height);

        RhythmNote note = newNoteGO.GetComponent<RhythmNote>();
        note.Initialize(randomKey, controller.hitZone);

        activeNotes.Add(note);
    }

    private void ScheduleNextNote()
    {
        nextNoteTime = Time.time + Random.Range(controller.difficultyData.piano_minTimeBetweenNotes, controller.difficultyData.piano_maxTimeBetweenNotes);
    }
}
