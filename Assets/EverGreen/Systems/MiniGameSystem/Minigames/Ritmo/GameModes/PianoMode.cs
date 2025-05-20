using System.Collections.Generic;
using UnityEngine;

public class PianoMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private float gameTimer;
    private float nextNoteTime;
    private List<PianoNoteData> activeNotes = new List<PianoNoteData>();
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
            PianoNoteData noteData = activeNotes[i];
            noteData.noteScript.MoveDown(controller.difficultyData.piano_noteSpeed);

            if (noteData.noteScript.IsOutOfBounds())
            {
                controller.feedbackText.text = "Errou!";
                GameObject.Destroy(noteData.noteGO);
                activeNotes.RemoveAt(i);
            }
        }
    }

    public void HandleInput(KeyCode key)
    {
        string pressedKey = key.ToString().ToUpper();

        PianoNoteData bestCandidate = null;
        float closestDistance = float.MaxValue;

        foreach (var noteData in activeNotes)
        {
            if (noteData.key.ToString().ToUpper() == pressedKey)
            {
                float distance = Mathf.Abs(noteData.noteScript.GetVerticalDistanceToHitZone());
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestCandidate = noteData;
                }
            }
        }

        if (bestCandidate != null)
        {
            string accuracy = EvaluateAccuracy(closestDistance);
            controller.feedbackText.text = accuracy;

            activeNotes.Remove(bestCandidate);
            GameObject.Destroy(bestCandidate.noteGO);
        }
        else
        {
            controller.feedbackText.text = "Errou!";
        }
    }

    private void SpawnNote()
    {
        char randomKey = controller.difficultyData.allowedKeys[Random.Range(0, controller.difficultyData.allowedKeys.Length)];
        GameObject newNoteGO = GameObject.Instantiate(controller.noteButtonPrefab, controller.noteArea);

        RectTransform noteRect = newNoteGO.GetComponent<RectTransform>();
        RectTransform areaRect = controller.noteArea.GetComponent<RectTransform>();

        float areaWidth = areaRect.rect.width;
        int keyIndex = controller.difficultyData.allowedKeys.IndexOf(randomKey);
        float slotWidth = areaWidth / controller.difficultyData.allowedKeys.Length;
        float x = -areaWidth / 2f + slotWidth * (keyIndex + 0.5f);

        noteRect.anchoredPosition = new Vector2(x, areaRect.rect.height / 2f + noteRect.rect.height);
        noteRect.localScale = Vector3.one;

        // Texto
        var tmpText = newNoteGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = randomKey.ToString();
        }

        // Cor
        var image = newNoteGO.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = Color.green;
        }

        var rhythmNote = newNoteGO.GetComponent<RhythmNote>();
        if (rhythmNote != null)
        {
            rhythmNote.Initialize(randomKey, controller.hitZone);
        }
        else
        {
            Debug.LogError("RhythmNote component not found in noteButtonPrefab!");
        }

        activeNotes.Add(new PianoNoteData
        {
            key = randomKey,
            noteGO = newNoteGO,
            noteScript = rhythmNote
        });
    }

    private string EvaluateAccuracy(float distance)
    {
        if (distance <= 10f) return "Perfeito!";
        if (distance <= 20f) return "Bom!";
        if (distance <= 30f) return "Ok!";
        if (distance <= 40f) return "Ruim!";
        return "Errou!";
    }

    private class PianoNoteData
    {
        public char key;
        public GameObject noteGO;
        public RhythmNote noteScript;
    }
}
