using UnityEngine;

[CreateAssetMenu(menuName = "Minigame/Rhythm/DifficultyData", fileName = "NewRhythmDifficultyData")]
public class RhythmMinigameDifficultyData : ScriptableObject
{
    [Header("Geral")]
    [Tooltip("Teclas permitidas para todos os modos.")]
    public string allowedKeys = "QWER";

    [Header("Classic Mode")]
    public float classic_minTimeBetweenNotes = 0.5f;
    public float classic_maxTimeBetweenNotes = 1.5f;
    public float classic_hitWindow = 0.3f;
    public float classic_gameDuration = 20f;

    [Header("Sequence Mode")]
    public int sequence_length = 5;
    public float sequence_displayInterval = 0.7f;

    [Header("Piano Mode")]
    public float piano_minTimeBetweenNotes = 0.5f;
    public float piano_maxTimeBetweenNotes = 1.5f;
    public float piano_noteSpeed = 300f;
    public float piano_gameDuration = 30f;
}
