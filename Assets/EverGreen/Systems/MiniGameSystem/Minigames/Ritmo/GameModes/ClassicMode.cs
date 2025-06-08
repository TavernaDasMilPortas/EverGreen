using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ClassicMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private MonoBehaviour coroutineExecutor;
    private List<RhythmNote> activeNotes = new List<RhythmNote>();
    private int correctHits;
    private int missedHits;
    private Coroutine countdownCoroutine;
    private Coroutine spawnNotesCoroutine;

    public bool IsModeFinished { get; private set; } = false;

    public ClassicMode(IRhythmGameController controller, MonoBehaviour coroutineExecutor)
    {
        this.controller = controller;
        this.coroutineExecutor = coroutineExecutor;
    }

    public void Initialize(IRhythmGameController controller)
    {
        this.controller = controller;
    }

    public void StartMode()
    {
        correctHits = 0;
        missedHits = 0;
        IsModeFinished = false;

        Debug.Log("[ClassicMode] StartMode iniciado");

        countdownCoroutine = coroutineExecutor.StartCoroutine(CountdownCoroutine());
        spawnNotesCoroutine = coroutineExecutor.StartCoroutine(SpawnNotesCoroutine());
    }

    public void UpdateMode()
    {
        // Não utilizado.
    }

    public void HandleInput(KeyCode key)
    {
        string pressedKey = key.ToString().ToLower();
        Debug.Log($"[ClassicMode] HandleInput chamado: Key pressed = {pressedKey}");

        RhythmNote hitNote = null;

        foreach (var note in activeNotes)
        {
            Debug.Log($"[ClassicMode] Checando nota ativa: {note.key.ToString().ToLower()}");
            if (note.key.ToString().ToLower() == pressedKey)
            {
                hitNote = note;
                break;
            }
        }

        if (hitNote != null)
        {
            controller.feedbackText.text = "Acertou!";
            Debug.Log("[ClassicMode] Acertou! FeedbackText atualizado.");
            correctHits++;

            activeNotes.Remove(hitNote);
            hitNote.DestroyNote();
        }
        else
        {
            controller.feedbackText.text = "Errou!";
            Debug.Log("[ClassicMode] Errou! FeedbackText atualizado.");
            missedHits++;

            if (activeNotes.Count > 0)
            {
                var lastNote = activeNotes[activeNotes.Count - 1];
                activeNotes.RemoveAt(activeNotes.Count - 1);
                lastNote.DestroyNote();
                Debug.Log("[ClassicMode] Última nota removida após erro.");
            }
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        float gameTimer = controller.difficultyData.classic_gameDuration;
        int lastDisplayedTime = -1;

        while (gameTimer > 0f)
        {
            gameTimer -= Time.deltaTime;

            int currentTime = Mathf.CeilToInt(gameTimer);
            if (currentTime != lastDisplayedTime)
            {
                controller.timerText.text = currentTime.ToString();
                lastDisplayedTime = currentTime;
                Debug.Log($"[ClassicMode] Timer atualizado: {currentTime}");
            }

            yield return null;
        }

        controller.timerText.text = "0";
        Debug.Log("[ClassicMode] Timer finalizado, chamando EndMode.");
        EndMode();
    }

    private IEnumerator SpawnNotesCoroutine()
    {
        while (!IsModeFinished)
        {
            float nextNoteTime = Random.Range(controller.difficultyData.classic_minTimeBetweenNotes, controller.difficultyData.classic_maxTimeBetweenNotes);
            Debug.Log($"[ClassicMode] Próxima nota em {nextNoteTime} segundos");
            yield return new WaitForSeconds(nextNoteTime);

            if (!IsModeFinished)
            {
                Debug.Log("[ClassicMode] SpawnNote chamado dentro do coroutine.");
                SpawnNote();
            }
        }
    }

    private void EndMode()
    {
        IsModeFinished = true;
        Debug.Log("[ClassicMode] EndMode chamado. Game Over");

        if (countdownCoroutine != null)
        {
            coroutineExecutor.StopCoroutine(countdownCoroutine);
            Debug.Log("[ClassicMode] CountdownCoroutine parado.");
        }
        if (spawnNotesCoroutine != null)
        {
            coroutineExecutor.StopCoroutine(spawnNotesCoroutine);
            Debug.Log("[ClassicMode] SpawnNotesCoroutine parado.");
        }

        controller.feedbackText.text = "";  //  OPCIONAL: limpa no fim também.
    }

    private void SpawnNote()
    {
        char randomKey = controller.difficultyData.allowedKeys[Random.Range(0, controller.difficultyData.allowedKeys.Length)];
        Transform parentTransform = GameObject.Find("MiniGamePanel").transform;

        Vector2 spawnPosition = GetRandomSpawnPosition(controller.noteArea.GetComponent<RectTransform>());

        Debug.Log($"[ClassicMode] Spawning note: key={randomKey}, position={spawnPosition}");

        GameObject noteGO = RhythmNote.SpawnNote(
            randomKey,
            controller.hitZone,
            controller.noteButtonPrefab,
            parentTransform,
            spawnPosition,
            NoteType.Classic,
            controller.difficultyData.classic_hitWindow
        );

        RhythmNote rhythmNote = noteGO.GetComponent<RhythmNote>();

        rhythmNote.StartLifeCycle(controller.difficultyData.classic_hitWindow, () =>
        {
            controller.feedbackText.text = "Errou!";
            Debug.Log("[ClassicMode] Nota não atingida a tempo. FeedbackText atualizado para 'Errou!'");
            missedHits++;
            activeNotes.Remove(rhythmNote);
        });

        rhythmNote.AnimateVisualOverLifetime(controller.difficultyData.classic_hitWindow);

        activeNotes.Add(rhythmNote);
    }

    private Vector2 GetRandomSpawnPosition(RectTransform areaRect)
    {
        float areaWidth = areaRect.rect.width;
        float noteWidth = controller.noteButtonPrefab.GetComponent<RectTransform>().rect.width;

        float x = Random.Range(-areaWidth / 2f + noteWidth / 2f, areaWidth / 2f - noteWidth / 2f);
        float y = 0f;

        return new Vector2(x, y);
    }
}
