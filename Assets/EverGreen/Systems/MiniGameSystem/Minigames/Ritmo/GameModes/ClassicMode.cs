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

    private bool timeIsUp = false; // NOVO: indica que o tempo acabou

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
        timeIsUp = false; // Resetar flag

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

        if (IsModeFinished)
            return;
        if (activeNotes.Count == 0)
        {
            Debug.Log("[ClassicMode] Nenhuma nota ativa. Input ignorado.");
            return;
        }
        // Permitir input mesmo se o tempo acabou, desde que haja notas ativas
        if (timeIsUp && activeNotes.Count == 0)
        {
            // Se tempo acabou e não há notas, ignora input
            return;
        }

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

            // Se o tempo acabou e essa foi a última nota, encerra o jogo
            if (timeIsUp && activeNotes.Count == 0)
            {
                Debug.Log("[ClassicMode] Tempo acabou e todas as notas foram processadas. Encerrando o jogo.");
                EndMode();
            }
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

            if (missedHits > controller.difficultyData.MissTolerance)
            {
                Debug.Log("[ClassicMode] Número de erros ultrapassou a tolerância. Encerrando o jogo.");
                EndMode();
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
        Debug.Log("[ClassicMode] Timer finalizado, tempo acabou, aguardando notas restantes.");

        timeIsUp = true;

        // Só finaliza se não houver notas restantes
        if (activeNotes.Count == 0)
        {
            EndMode();
        }
    }

    private IEnumerator SpawnNotesCoroutine()
    {
        while (!IsModeFinished && !timeIsUp) // Parar spawn após o tempo acabar
        {
            float nextNoteTime = Random.Range(controller.difficultyData.classic_minTimeBetweenNotes, controller.difficultyData.classic_maxTimeBetweenNotes);
            Debug.Log($"[ClassicMode] Próxima nota em {nextNoteTime} segundos");
            yield return new WaitForSeconds(nextNoteTime);

            if (!IsModeFinished && !timeIsUp)
            {
                Debug.Log("[ClassicMode] SpawnNote chamado dentro do coroutine.");
                SpawnNote();
            }
        }
    }

    private void EndMode()
    {
        if (IsModeFinished) return;

        IsModeFinished = true;
        Debug.Log("[ClassicMode] EndMode chamado. Game Over");

        // Apaga todas as notas ativas restantes antes de terminar o jogo
        foreach (var note in activeNotes)
        {
            note.DestroyNote();
        }
        activeNotes.Clear();

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
        if (missedHits > controller.difficultyData.MissTolerance)
        {
            controller.gameResult = false;
        }
        else
        {
            controller.gameResult = true;
        }
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

            // Verifica se passou da tolerância
            if (missedHits > controller.difficultyData.MissTolerance)
            {
                Debug.Log("[ClassicMode] Número de erros ultrapassou a tolerância. Encerrando o jogo.");
                EndMode();
            }

            // Se tempo acabou e não tem mais notas, finaliza
            if (timeIsUp && activeNotes.Count == 0)
            {
                Debug.Log("[ClassicMode] Tempo acabou e todas as notas foram processadas. Encerrando o jogo.");
                EndMode();
            }
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
