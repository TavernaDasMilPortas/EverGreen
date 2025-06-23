using System.Collections.Generic;
using UnityEngine;
public class PianoMode : IRhythmGameMode
{
    public enum NoteAccuracy
    {
        Perfect,
        Good,
        Okay,
        Bad,
        Miss
    }
    private IRhythmGameController controller;
    private float gameTimer;
    private float nextNoteTime;
    private List<RhythmNote> activeNotes = new List<RhythmNote>();

    public bool IsModeFinished { get; private set; } = false;

    private float successfulHits = 0;
    private float missedHits = 0;

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
        successfulHits = 0;
        missedHits = 0;
        IsModeFinished = false;
    }

    public void UpdateMode()
    {
        if (IsModeFinished) return;

        gameTimer -= Time.deltaTime;
        controller.timerText.text = Mathf.CeilToInt(gameTimer).ToString();

        if (gameTimer <= 0)
        {
            EndMode();
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
                missedHits++;
                note.DestroyNote();
                activeNotes.RemoveAt(i);
            }
        }
    }

    public void HandleInput(KeyCode key)
    {
        RhythmNote bestCandidate = null;
        float closestDistance = float.MaxValue;

        foreach (var note in activeNotes)
        {
            if (note.MatchesKey(key))
            {
                float distance = note.DistanceToHitArea();

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestCandidate = note;
                }
            }
        }

        if (bestCandidate != null)
        {
            NoteAccuracy accuracy = EvaluateAccuracy(closestDistance);
            controller.feedbackText.text = GetAccuracyText(accuracy);

            switch (accuracy)
            {
                case NoteAccuracy.Perfect:
                    successfulHits+=1.5f;
                    break;
                case NoteAccuracy.Good:
                    successfulHits++;
                    break;
                case NoteAccuracy.Okay:
                    successfulHits+=0.5f;
                    break;
                case NoteAccuracy.Bad:
                    missedHits+= 0.5f;
                    break;
                case NoteAccuracy.Miss:
                    missedHits++;
                    break;
            }

            activeNotes.Remove(bestCandidate);
            bestCandidate.OnHit();
        }
        else
        {
            controller.feedbackText.text = "Errou!";
            missedHits++;
        }
    }


    private void EndMode()
    {
        if (IsModeFinished) return;

        IsModeFinished = true;

        Debug.Log("[PianoMode] EndMode chamado.");

        foreach (var note in activeNotes)
        {
            note.DestroyNote();
        }

        activeNotes.Clear();

        int missTolerance = controller.difficultyData.MissTolerancePiano;
        int minHitsToWin = controller.difficultyData.minSuccessHits;

        // Critério: Erros excederam a tolerância ou acertos abaixo do mínimo
        if (missedHits > missTolerance || successfulHits < minHitsToWin)
        {
            controller.gameResult = false;
            Debug.Log($"[PianoMode] Derrota - Erros: {missedHits}, Acertos: {successfulHits}");
        }
        else
        {
            controller.gameResult = true;
            Debug.Log($"[PianoMode] Vitória - Erros: {missedHits}, Acertos: {successfulHits}");
        }
    }

    private void SpawnNote()
    {
        char randomKey = controller.difficultyData.allowedKeys[Random.Range(0, controller.difficultyData.allowedKeys.Length)];
        RectTransform areaRect = controller.noteArea.GetComponent<RectTransform>();
        float areaWidth = areaRect.rect.width;
        int keyIndex = controller.difficultyData.allowedKeys.IndexOf(randomKey);
        float slotWidth = areaWidth / controller.difficultyData.allowedKeys.Length;
        float x = -areaWidth / 2f + slotWidth * (keyIndex + 0.5f);
        Vector2 spawnPos = new Vector2(x, areaRect.rect.height / 2f + 50f);
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
        if (rhythmNote != null)
        {
            activeNotes.Add(rhythmNote);
        }
    }

    private NoteAccuracy EvaluateAccuracy(float distance)
    {
        if (distance <= 10f) return NoteAccuracy.Perfect;
        if (distance <= 20f) return NoteAccuracy.Good;
        if (distance <= 30f) return NoteAccuracy.Okay;
        if (distance <= 40f) return NoteAccuracy.Bad;
        return NoteAccuracy.Miss;
    }

    private string GetAccuracyText(NoteAccuracy accuracy)
    {
        switch (accuracy)
        {
            case NoteAccuracy.Perfect: return "Perfeito!";
            case NoteAccuracy.Good: return "Bom!";
            case NoteAccuracy.Okay: return "Ok!";
            case NoteAccuracy.Bad: return "Ruim!";
            case NoteAccuracy.Miss: return "Errou!";
            default: return "";
        }
    }

}
