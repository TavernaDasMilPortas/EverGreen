using System.Collections.Generic;
using UnityEngine;

public class ClassicMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private float gameTimer;
    private float nextNoteTime;
    private List<ClassicNoteData> activeNotes = new List<ClassicNoteData>();
    private int correctHits;
    private int missedHits;
    public bool IsModeFinished { get; private set; } = false;

    public ClassicMode(IRhythmGameController controller)
    {
        this.controller = controller;
    }
    public void Initialize(IRhythmGameController controller)
    {
        this.controller = controller;
    }

    public void StartMode()
    {
        gameTimer = controller.difficultyData.classic_gameDuration;
        correctHits = 0;
        missedHits = 0;
        nextNoteTime = Random.Range(controller.difficultyData.classic_minTimeBetweenNotes, controller.difficultyData.classic_maxTimeBetweenNotes);
        // Inicializa o tempo para a próxima nota com um valor relativo, não absoluto
    }

    public void UpdateMode()
    {
        gameTimer -= Time.deltaTime;
        controller.timerText.text = Mathf.CeilToInt(gameTimer).ToString();



        if (gameTimer <= 0)
        {
            IsModeFinished = true;
            Debug.Log("[UpdateMode] Game Over");
            return;
        }

        nextNoteTime -= Time.deltaTime;


        if (nextNoteTime <= 0f)
        {
            Debug.Log("[UpdateMode] Spawning new note...");
            SpawnNote();
            nextNoteTime = Random.Range(controller.difficultyData.classic_minTimeBetweenNotes, controller.difficultyData.classic_maxTimeBetweenNotes);

        }

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            ClassicNoteData noteData = activeNotes[i];
            float timeLeft = noteData.expireTime - Time.time;

            Debug.Log($"[UpdateMode] Note '{noteData.key}' time left: {timeLeft:F2}");

            if (Time.time >= noteData.expireTime)
            {
                controller.feedbackText.text = "Errou!";

                GameObject.Destroy(noteData.noteGO);
                activeNotes.RemoveAt(i);
                missedHits++;
            }
        }
    }

    public void HandleInput(KeyCode key)
    {
        string pressedKey = key.ToString().ToUpper();
        Debug.Log($"[HandleInput] Key pressed: {pressedKey}");

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

            // Se houver pelo menos uma nota ativa, remova a última
            if (activeNotes.Count > 0)
            {
                var lastNote = activeNotes[activeNotes.Count - 1];
                activeNotes.RemoveAt(activeNotes.Count - 1);
                GameObject.Destroy(lastNote.noteGO);
            }
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

        float y = 0f;
        noteRect.anchoredPosition = new Vector2(x, y);

        // Inicializa com escala 2x
        noteRect.localScale = Vector3.one * 2f;

        // TextMeshProUGUI da nota
        var tmpText = newNoteGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = randomKey.ToString();
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in noteButtonPrefab!");
        }

        // Indicador visual
        var indicatorImage = newNoteGO.GetComponent<UnityEngine.UI.Image>();
        if (indicatorImage != null)
        {
            indicatorImage.color = Color.green;
        }

        // Referência ao RhythmNote
        var rhythmNote = newNoteGO.GetComponent<RhythmNote>();
        if (rhythmNote != null)
        {
            rhythmNote.Initialize(randomKey, controller.noteArea);
        }
        else
        {
            Debug.LogError("RhythmNote component not found in noteButtonPrefab!");
        }

        float expire = Time.time + controller.difficultyData.classic_hitWindow;


        activeNotes.Add(new ClassicNoteData
        {
            key = randomKey,
            noteGO = newNoteGO,
            noteScript = rhythmNote,
            expireTime = expire,
            spawnTime = Time.time,
            lifeTime = controller.difficultyData.classic_hitWindow,
            indicatorImage = indicatorImage
        });

        if (rhythmNote != null)
        {
            rhythmNote.Initialize(randomKey, controller.noteArea);
            rhythmNote.rectTransform = noteRect;
            rhythmNote.AnimateVisualOverLifetime(controller.difficultyData.classic_hitWindow);
        }
    }


    private class ClassicNoteData
    {
        public char key;
        public GameObject noteGO;
        public RhythmNote noteScript;
        public float expireTime;
        public float spawnTime;
        public float lifeTime;
        public UnityEngine.UI.Image indicatorImage;
    }
}
