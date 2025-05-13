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
    public TextMeshProUGUI timerText;

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
        Debug.Log("Minigame iniciado.");
    }

    public void UpdateMinigame()
    {
        if (!isRunning) return;

        gameTimer -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(gameTimer).ToString();

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
                Debug.Log($"Nota expirada! MissedHits: {missedHits + 1}");
                Destroy(note.gameObject);
                activeNotes.RemoveAt(i);
                missedHits++;
            }
        }

        if (gameTimer <= 0)
        {
            isRunning = false;
            Debug.Log("Minigame finalizado. Avaliando resultado...");
            MinigameManager.Instance.EndMinigame();
        }
    }

    public void HandleInput(KeyCode key)
    {
        string pressedKey = key.ToString().ToUpper(); // Normaliza a tecla pressionada
        Debug.Log($"Tecla pressionada: {pressedKey}");

        if (activeNotes.Count == 0)
        {
            feedbackText.text = "Errou!";
            missedHits++;
            Debug.Log("Nenhuma nota ativa. MissedHits incrementado.");
            return;
        }

        // Remove e avalia apenas a nota mais antiga
        RhythmNote note = activeNotes[0];
        activeNotes.RemoveAt(0);
        Destroy(note.gameObject);

        string noteKey = note.GetKey().ToString().ToUpper(); // Normaliza a tecla da nota
        Debug.Log($"Comparando tecla: {pressedKey} com nota: {noteKey}");

        if (pressedKey == noteKey && note.IsWithinHitWindow(difficultyData.hitWindow))
        {
            feedbackText.text = "Acertou!";
            correctHits++;
            Debug.Log($"Nota correta! Total acertos: {correctHits}");
        }

        feedbackText.text = "Errou!";
        missedHits++;
    }

    public void EndMinigame()
    {
        Debug.Log("Encerrando minigame e limpando notas.");
        foreach (var note in activeNotes)
        {
            Destroy(note.gameObject);
        }
        activeNotes.Clear();
    }

    public bool EvaluateResult()
    {
        Debug.Log($"Resultado - Acertos: {correctHits}, Erros: {missedHits}");
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

        Debug.Log($"Nota gerada com tecla '{randomKey}' na posição ({x}, {y}). Total de notas: {totalNotes}");
    }
}
