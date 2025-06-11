using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RhythmGameController : MonoBehaviour, IRhythmGameController, IMinigame
{
    [Header("Configurações")]
    [SerializeField] private RhythmMinigameDifficultyData _difficultyData;
    [SerializeField] public GameObject _notePrefab;
    [SerializeField] public RectTransform _noteArea;
    [SerializeField] public TextMeshProUGUI _feedbackText;
    [SerializeField] public GameObject[] UiRoot;
    private Transform parentUi;
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
    private bool _gameResult = false;

    // Implementação da propriedade da interface
    public bool gameResult
    {
        get => _gameResult;
        set => _gameResult = value;
    }
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

    public void AssignUI(GameObject[] uiRoots)
    {
        parentUi = GameObject.Find("MiniGamePanel").transform;
        Debug.Log($"[RhythmGameController] Iniciando AssignUI com {uiRoots.Length} elementos. 1- {uiRoots[0]} 2- {uiRoots[1]}, 3- {uiRoots[2]}, 4- {uiRoots[3]}");
        UiRoot = uiRoots;
        foreach (GameObject uiRoot in uiRoots)
        {
            if (_noteArea == null)
            {
                RectTransform noteAreaTransform = FindDeepChild(parentUi, "NoteArea") as RectTransform;
                if (noteAreaTransform != null)
                {
                    _noteArea = noteAreaTransform;
                    Debug.Log($"[RhythmGameController] NoteArea encontrado: {GetPath(noteAreaTransform)}");
                }
            }

            if (_feedbackText == null)
            {
                Transform feedbackTextTransform = FindDeepChild(parentUi, "FeedbackText");
                if (feedbackTextTransform != null)
                {
                    _feedbackText = feedbackTextTransform.GetComponent<TextMeshProUGUI>();
                    Debug.Log($"[RhythmGameController] FeedbackText encontrado: {GetPath(feedbackTextTransform)}");
                }
            }

            if (_hitZone == null)
            {
                Transform hitZoneTransform = FindDeepChild(parentUi, "HitZone");
                if (hitZoneTransform != null)
                {
                    _hitZone = hitZoneTransform.GetComponent<RectTransform>();
                    Debug.Log($"[RhythmGameController] HitZone encontrado: {GetPath(hitZoneTransform)}");
                }
            }

            if (_timerText == null)
            {
                Transform timerTextTransform = FindDeepChild(parentUi, "TimerText");
                if (timerTextTransform != null)
                {
                    _timerText = timerTextTransform.GetComponent<TextMeshProUGUI>();
                    Debug.Log($"[RhythmGameController] TimerText encontrado: {GetPath(timerTextTransform)}");
                }
            }

            // Se todos já foram encontrados, podemos encerrar o loop
            if (_noteArea != null && _feedbackText != null && _hitZone != null && _timerText != null)
                break;
        }

        // Relatórios de erros caso algum ainda não tenha sido encontrado
        if (_noteArea == null)
            Debug.LogError("[RhythmGameController] NoteArea NÃO encontrado em nenhum UI.");

        if (_feedbackText == null)
            Debug.LogError("[RhythmGameController] FeedbackText NÃO encontrado em nenhum UI.");

        if (_hitZone == null)
            Debug.LogError("[RhythmGameController] HitZone NÃO encontrado em nenhum UI.");

        if (_timerText == null)
            Debug.LogError("[RhythmGameController] TimerText NÃO encontrado em nenhum UI.");
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
            if (child.name.StartsWith(childName))
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
 
        }
        SetMode(RhythmGameMode.CreateMode(selectedMode, this, this));
        currentMode?.StartMode();
        modeStarted = true;

        Debug.Log("[RhythmGameController] StartMinigame called.");
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
        Debug.Log("[RhythmGameController] EndMinigame called.");
        modeStarted = false;
        activeNotes.ForEach(note => Destroy(note.gameObject));
        activeNotes.Clear();
        MinigameManager.Instance.gameFinish = true;
    }

    public bool EvaluateResult()
    {
        // Exemplo básico (ajuste conforme sua lógica)
        return gameResult;
    }

}
