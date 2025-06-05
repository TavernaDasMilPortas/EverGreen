using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RhythmGameController : MonoBehaviour, IRhythmGameController, IMinigame
{
    [Header("Configurações")]
    [SerializeField] private RhythmMinigameDifficultyData _difficultyData;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private RectTransform _noteArea;
    [SerializeField] private TextMeshProUGUI _feedbackText;
    [SerializeField] private GameObject UiRoot;

    [Header("Modo de Jogo")]
    public GameModes.Modes selectedMode;

    private IRhythmGameMode currentMode;
    private NoteSpawnerService spawnerService;
    private NoteEvaluatorService evaluatorService;

    private List<RhythmNote> activeNotes = new List<RhythmNote>();

    [Header("UI Extras")]
    //[SerializeField] private GameObject _gamePrefab; // Prefab do minigame, usado para ativar/desativa
    private TextMeshProUGUI _timerText;
    private RectTransform _hitZone;

    private bool modeStarted = false;  // controla se o jogo está rodando
    //public GameObject gamePrefab => _gamePrefab;

    public RectTransform hitZone => _hitZone;
    public GameObject notePrefab => _notePrefab;
    public RectTransform noteArea => _noteArea;
    public TextMeshProUGUI feedbackText => _feedbackText;
    public GameObject noteButtonPrefab => _notePrefab;
    public TextMeshProUGUI timerText => _timerText;

    public RhythmMinigameDifficultyData difficultyData => _difficultyData;

  
    private void Start()
    {
        spawnerService = new NoteSpawnerService();
        evaluatorService = new NoteEvaluatorService(); 

        // Não iniciar modo automaticamente
        // SetMode(RhythmGameMode.CreateMode(selectedMode, this));
    }

    public void SetMode(IRhythmGameMode mode)
    {
        currentMode = mode;
        currentMode.Initialize(this);
        modeStarted = false; // resetar flag ao trocar de modo
    }

    public void SelectMode( GameModes.Modes mode)
    {
        selectedMode = mode;
    }

    public void SetDifficulty(IDifficultData Difficult)
    {
        RhythmMinigameDifficultyData rhythmDifficulty = Difficult as RhythmMinigameDifficultyData;
        if (rhythmDifficulty != null)
        {
            _difficultyData = rhythmDifficulty;
        }
        else
        {
            Debug.LogError("Dificuldade fornecida não é do tipo RhythmMinigameDifficultyData!");
        }
    }
    public void SpawnNote(char key, Vector2 position)
    {
        var note = spawnerService.SpawnNote(notePrefab, noteArea, key, position);
        activeNotes.Add(note);
    }

    public void CheckNoteHit(KeyCode key)
    {
        foreach (var note in activeNotes)
        {
            if (note.MatchesKey(key))
            {
                var result = evaluatorService.EvaluateHit(note);
                feedbackText.text = result.ToString();
                activeNotes.Remove(note);
                Destroy(note.gameObject);
                return;
            }
        }
        feedbackText.text = "Miss!";
    }

    public void AssignUI(GameObject uiRoot)
    {
        Debug.Log($"[RhythmGameController] Iniciando AssignUI no UiRoot: {uiRoot.name}");

        Transform noteAreaTransform = FindDeepChild(uiRoot.transform, "NoteArea");
        _noteArea = noteAreaTransform?.GetComponent<RectTransform>();
        if (_noteArea != null)
            Debug.Log($"[RhythmGameController] NoteArea encontrado: {GetPath(noteAreaTransform)}");
        else
            Debug.LogError($"[RhythmGameController] NoteArea NÃO encontrado no UiRoot: {uiRoot.name}");

        Transform feedbackTextTransform = FindDeepChild(uiRoot.transform, "FeedbackText");
        _feedbackText = feedbackTextTransform?.GetComponent<TextMeshProUGUI>();
        if (_feedbackText != null)
            Debug.Log($"[RhythmGameController] FeedbackText encontrado: {GetPath(feedbackTextTransform)}");
        else
            Debug.LogError($"[RhythmGameController] FeedbackText NÃO encontrado no UiRoot: {uiRoot.name}");

        Transform hitZoneTransform = FindDeepChild(uiRoot.transform, "HitZone");
        _hitZone = hitZoneTransform?.GetComponent<RectTransform>();
        if (_hitZone != null)
            Debug.Log($"[RhythmGameController] HitZone encontrado: {GetPath(hitZoneTransform)}");
        else
            Debug.LogError($"[RhythmGameController] HitZone NÃO encontrado no UiRoot: {uiRoot.name}");

        Transform timerTextTransform = FindDeepChild(uiRoot.transform, "TimerText");
        _timerText = timerTextTransform?.GetComponent<TextMeshProUGUI>();
        if (_timerText != null)
            Debug.Log($"[RhythmGameController] TimerText encontrado: {GetPath(timerTextTransform)}");
        else
            Debug.LogError($"[RhythmGameController] TimerText NÃO encontrado no UiRoot: {uiRoot.name}");
    }
    private string GetPath(Transform obj)
    {
        if (obj == null) return "null";
        string path = obj.name;
        while (obj.parent != null)
        {
            obj = obj.parent;
            path = obj.name + "/" + path;
        }
        return path;
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }

    public List<RhythmNote> GetActiveNotes() => activeNotes;

    // Implementação da interface IMinigame
    public void StartMinigame()
    {
        if (UiRoot == null)
        {
            Debug.LogWarning("UiRoot não atribuído. AssignUI não será chamado.");
        }
        else
        {
            AssignUI(UiRoot);
        }
        SetMode(RhythmGameMode.CreateMode(selectedMode, this, this));
        currentMode?.StartMode();
        modeStarted = true;
        // Quem chamar StartMinigame deve chamar BeginMinigame para começar
    }

    public void UpdateMinigame()
    {
        if (!modeStarted || currentMode == null) return;

        currentMode.UpdateMode();

        if (currentMode.IsModeFinished)
        {
            Debug.Log("Minigame finished by mode.");
            EndMinigame();
        }
    }

    public void HandleInput(KeyCode key)
    {
        currentMode?.HandleInput(key);
    }

    public void EndMinigame()
    {
        Debug.Log("Rhythm Minigame Ended.");
        modeStarted = false;
        activeNotes.ForEach(note => Destroy(note.gameObject));
        activeNotes.Clear();
    }

    public bool EvaluateResult()
    {
        // Exemplo básico (ajuste conforme sua lógica)
        return true;
    }
}
