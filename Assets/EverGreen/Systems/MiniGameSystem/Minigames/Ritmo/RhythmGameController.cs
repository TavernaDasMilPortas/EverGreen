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
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField]private RectTransform _hitZone;

    private bool modeStarted = false;

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
        AssignUI(UiRoot);
    }

    private void Update()
    {
        if (!modeStarted || currentMode == null) return;

        currentMode.UpdateMode();

        if (currentMode.IsModeFinished)
        {
            Debug.Log("Minigame finished by mode.");
            EndMinigame();
        }

        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    currentMode.HandleInput(key);
                    break;
                }
            }
        }
    }

    public void SetMode(IRhythmGameMode mode)
    {
        currentMode = mode;
        currentMode.Initialize(this);
        modeStarted = false;

        Debug.Log($"[RhythmGameController] SetMode: {mode.GetType().Name}");
    }

    public void SelectMode(GameModes.Modes mode)
    {
        selectedMode = mode;
        Debug.Log($"[RhythmGameController] SelectMode: {selectedMode}");
    }

    public void SetDifficulty(IDifficultData Difficult)
    {
        RhythmMinigameDifficultyData rhythmDifficulty = Difficult as RhythmMinigameDifficultyData;
        if (rhythmDifficulty != null)
        {
            _difficultyData = rhythmDifficulty;
            Debug.Log($"[RhythmGameController] SetDifficulty: {_difficultyData.name}");
        }
        else
        {
            Debug.LogError("[RhythmGameController] SetDifficulty: Dificuldade fornecida não é do tipo RhythmMinigameDifficultyData!");
        }
    }

    public void BeginMinigame()
    {
        if (currentMode == null)
        {
            SetMode(RhythmGameMode.CreateMode(selectedMode, this));
        }
        currentMode?.StartMode();
        modeStarted = true;

        Debug.Log("[RhythmGameController] BeginMinigame called.");
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
        Transform noteAreaTransform = FindDeepChild(uiRoot.transform, "NoteArea");
        _noteArea = noteAreaTransform?.GetComponent<RectTransform>();
        if (_noteArea != null)
            Debug.Log("[RhythmGameController] NoteArea encontrado.");
        else
            Debug.LogWarning("[RhythmGameController] NoteArea NÃO encontrado.");

        Transform feedbackTextTransform = FindDeepChild(uiRoot.transform, "FeedbackText");
        _feedbackText = feedbackTextTransform?.GetComponent<TextMeshProUGUI>();
        if (_feedbackText != null)
            Debug.Log("[RhythmGameController] FeedbackText encontrado.");
        else
            Debug.LogWarning("[RhythmGameController] FeedbackText NÃO encontrado.");

        Transform hitZoneTransform = FindDeepChild(uiRoot.transform, "HitZone");
        _hitZone = hitZoneTransform?.GetComponent<RectTransform>();
        if (_hitZone != null)
            Debug.Log("[RhythmGameController] HitZone encontrado.");
        else
            Debug.LogWarning("[RhythmGameController] HitZone NÃO encontrado.");

        Transform timerTextTransform = FindDeepChild(uiRoot.transform, "TimerText");
        _timerText = timerTextTransform?.GetComponent<TextMeshProUGUI>();
        if (_timerText != null)
            Debug.Log("[RhythmGameController] TimerText encontrado.");
        else
            Debug.LogWarning("[RhythmGameController] TimerText NÃO encontrado.");
    }


    public List<RhythmNote> GetActiveNotes() => activeNotes;

    // Implementação da interface IMinigame
    public void StartMinigame()
    {
        SetMode(RhythmGameMode.CreateMode(selectedMode, this));
        currentMode?.StartMode();
        modeStarted = true;

        Debug.Log("[RhythmGameController] StartMinigame called.");
    }

    public void UpdateMinigame()
    {
        Update();
    }

    public void HandleInput(KeyCode key)
    {
        currentMode?.HandleInput(key);
    }

    public void EndMinigame()
    {
        Debug.Log("[RhythmGameController] EndMinigame called.");
        modeStarted = false;
        activeNotes.ForEach(note => Destroy(note.gameObject));
        activeNotes.Clear();
    }

    public bool EvaluateResult()
    {
        return true;
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
}
