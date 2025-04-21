using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RhythmMinigameController : MonoBehaviour, IMinigame
{
    [Header("Referências da Cena")]
    public Transform noteArea;
    public GameObject noteButtonPrefab;
    public TextMeshProUGUI feedbackText;

    [Header("Configuração")]
    public RhythmMinigameDifficultyData difficultyData;

    private float gameTimer;
    private List<RhythmNote> activeNotes = new List<RhythmNote>();
    private bool isRunning = false;
    private float nextNoteTime;

    private int totalNotes;
    private int correctHits;
    private int missedHits;

    public void StartMinigame()
    {
        gameTimer = difficultyData.gameDuration;
        isRunning = true;
        correctHits = 0;
        missedHits = 0;
        nextNoteTime = Time.time + Random.Range(difficultyData.minTimeBetweenNotes, difficultyData.maxTimeBetweenNotes);
    }

    public void UpdateMinigame()
    {
        if (!isRunning) return;

        gameTimer -= Time.deltaTime;

        if (Time.time >= nextNoteTime)
        {
            SpawnNote();
            nextNoteTime = Time.time + Random.Range(difficultyData.minTimeBetweenNotes, difficultyData.maxTimeBetweenNotes);
        }

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            RhythmNote note = activeNotes[i];
            note.UpdateNote(difficultyData.noteSpeed);

            if (note.HasExpired())
            {
                feedbackText.text = "Errou!";
                Destroy(note.gameObject);
                activeNotes.RemoveAt(i);
                missedHits++;
            }
        }

        if (gameTimer <= 0)
        {
            isRunning = false;
            MinigameManager.Instance.EndMinigame();
        }
    }

    public void HandleInput(KeyCode key)
    {
        for (int i = 0; i < activeNotes.Count; i++)
        {
            RhythmNote note = activeNotes[i];
            if (note.MatchesKey(key) && note.IsWithinHitWindow(difficultyData.hitWindow))
            {
                feedbackText.text = "Acertou!";
                correctHits++;
                Destroy(note.gameObject);
                activeNotes.RemoveAt(i);
                return;
            }
        }

        feedbackText.text = "Errou!";
        missedHits++;
    }

    public void EndMinigame()
    {
        foreach (var note in activeNotes)
        {
            Destroy(note.gameObject);
        }
        activeNotes.Clear();
    }

    public bool EvaluateResult()
    {
        return correctHits > missedHits;
    }

    private void SpawnNote()
    {
        char randomKey = difficultyData.allowedKeys[Random.Range(0, difficultyData.allowedKeys.Length)];
        GameObject newNoteGO = Instantiate(noteButtonPrefab, noteArea);
        RhythmNote note = newNoteGO.GetComponent<RhythmNote>();
        note.Initialize(randomKey);
        activeNotes.Add(note);
        totalNotes++;
    }
}
