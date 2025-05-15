using UnityEngine;
using TMPro;
using System.Collections.Generic;


public interface IRhythmGameController
{
    RhythmMinigameDifficultyData difficultyData { get; }
    GameObject notePrefab { get; }
    GameObject noteButtonPrefab { get; }
    RectTransform noteArea { get; }
    TextMeshProUGUI feedbackText { get; }
    TextMeshProUGUI timerText { get; }
    RectTransform hitZone { get; }
    void SpawnNote(char key, Vector2 position);
    void CheckNoteHit(KeyCode key);
    System.Collections.Generic.List<RhythmNote> GetActiveNotes();
}
