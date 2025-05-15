using System.Collections.Generic;
using UnityEngine;

public class ClassicMode : IRhythmGameMode
{
    private RhythmMinigameController controller;
    private float gameTimer;
    private float nextNoteTime;
    private List<ClassicNoteData> activeNotes = new List<ClassicNoteData>();
    private int correctHits;
    private int missedHits;
    public bool IsModeFinished { get; private set; } = false;

    public ClassicMode(RhythmMinigameController controller)
    {
        this.controller = controller;
    }

    public void StartMode()
    {
        gameTimer = controller.difficultyData.classic_gameDuration;
        correctHits = 0;
        missedHits = 0;
        nextNoteTime = Time.time + Random.Range(controller.difficultyData.classic_minTimeBetweenNotes, controller.difficultyData.classic_maxTimeBetweenNotes);
    }

    public void UpdateMode()
    {
        gameTimer -= Time.deltaTime;
        controller.timerText.text = Mathf.CeilToInt(gameTimer).ToString();

        if (Time.time >= nextNoteTime)
        {
            SpawnNote();
            nextNoteTime = Time.time + Random.Range(controller.difficultyData.classic_minTimeBetweenNotes, controller.difficultyData.classic_maxTimeBetweenNotes);
        }

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            ClassicNoteData noteData = activeNotes[i];

            if (Time.time >= noteData.expireTime)
            {
                controller.feedbackText.text = "Errou!";
                GameObject.Destroy(noteData.noteGO);
                activeNotes.RemoveAt(i);
                missedHits++;
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
        ClassicNoteData hitNote = null;

        foreach (var noteData in activeNotes)
        {
            if (noteData.key.ToString().ToUpper() == pressedKey)
            {
                hitNote = noteData;
                break;
            }
        }

        if (hitNote != null)
        {
            controller.feedbackText.text = "Acertou!";
            correctHits++;
            activeNotes.Remove(hitNote);
            GameObject.Destroy(hitNote.noteGO);
        }
        else
        {
            controller.feedbackText.text = "Errou!";
            missedHits++;
        }
    }

    private void SpawnNote()
    {
        char randomKey = controller.difficultyData.allowedKeys[Random.Range(0, controller.difficultyData.allowedKeys.Length)];
        GameObject newNoteGO = GameObject.Instantiate(controller.noteButtonPrefab, controller.noteArea);

        RectTransform noteRect = newNoteGO.GetComponent<RectTransform>();
        RectTransform areaRect = controller.noteArea.GetComponent<RectTransform>();

        float areaWidth = areaRect.rect.width;
        float x = Random.Range(-areaWidth / 2f + noteRect.rect.width / 2f, areaWidth / 2f - noteRect.rect.width / 2f);

        float y = 0f; // fixo no centro (pode ajustar se quiser lanes)
        noteRect.anchoredPosition = new Vector2(x, y);

        newNoteGO.GetComponentInChildren<UnityEngine.UI.Text>().text = randomKey.ToString();

        activeNotes.Add(new ClassicNoteData
        {
            key = randomKey,
            noteGO = newNoteGO,
            expireTime = Time.time + controller.difficultyData.classic_hitWindow
        });
    }

    private class ClassicNoteData
    {
        public char key;
        public GameObject noteGO;
        public float expireTime;
    }
}
