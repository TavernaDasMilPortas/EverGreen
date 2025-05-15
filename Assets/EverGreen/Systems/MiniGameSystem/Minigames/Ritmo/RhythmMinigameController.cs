using TMPro;
using UnityEngine;

public class RhythmMinigameController : MonoBehaviour, IMinigame
{
    [Header("Refer�ncias da Cena")]
    public Transform noteArea;
    public GameObject noteButtonPrefab;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI timerText;

    [Header("Configura��o")]
    public RhythmMinigameDifficultyData difficultyData;
    public RhythmGameModeType selectedMode;

    private IRhythmGameMode currentMode;

    public void StartMinigame()
    {
        currentMode = RhythmGameModeFactory.CreateMode(selectedMode, this);
        currentMode.StartMode();
    }

    public void UpdateMinigame()
    {
        if (currentMode != null && !currentMode.IsModeFinished)
        {
            currentMode.UpdateMode();
        }
        else
        {
            MinigameManager.Instance.EndMinigame();
        }
    }

    public void HandleInput(KeyCode key)
    {
        currentMode?.HandleInput(key);
    }

    public void EndMinigame()
    {
        // Limpar notas ou estados globais aqui se necess�rio
    }

    public bool EvaluateResult()
    {
        // Pode delegar para o modo se precisar de regras espec�ficas
        return true;
    }
}
