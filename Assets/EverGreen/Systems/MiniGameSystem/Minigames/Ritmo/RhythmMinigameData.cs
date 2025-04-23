using UnityEngine;

[CreateAssetMenu(menuName = "Minigame/Rhythm/DifficultyData", fileName = "NewRhythmDifficultyData")]
public class RhythmMinigameDifficultyData : ScriptableObject
{
    [Header("Teclas permitidas para notas")]
    public string allowedKeys = "qwer";

    [Header("Tempo entre notas")]
    public float minTimeBetweenNotes = 0.5f;
    public float maxTimeBetweenNotes = 1.5f;

    [Header("Velocidade das notas")]
    public float noteSpeed = 1.0f;

    [Header("Tempo total do minigame")]
    public float gameDuration = 20f;

    [Header("Tolerância de acerto (segundos)")]
    public float hitWindow = 0.3f;
}