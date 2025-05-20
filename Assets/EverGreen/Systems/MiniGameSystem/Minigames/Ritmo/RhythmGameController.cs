using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RhythmGameController : MonoBehaviour, IRhythmGameController, IMinigame
{
    [Header("Configura��es")]
    [SerializeField] private RhythmMinigameDifficultyData _difficultyData;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private RectTransform _noteArea;
    [SerializeField] private TextMeshProUGUI _feedbackText;

    [Header("Modo de Jogo")]
    public RhythmGameMode.RhythmGameModeType selectedMode;

    private IRhythmGameMode currentMode;
    private NoteSpawnerService spawnerService;
    private NoteEvaluatorService evaluatorService;

    private List<RhythmNote> activeNotes = new List<RhythmNote>();

    [Header("UI Extras")]
    [SerializeField] private GameObject _noteButtonPrefab;
    [SerializeField] private TextMeshProUGUI _timerText;

    private bool modeStarted = false;  // controla se o jogo est� rodando

    public RectTransform hitZone { get; }
    // Interface properties
    public RhythmMinigameDifficultyData difficultyData => _difficultyData;
    public GameObject notePrefab => _notePrefab;
    public RectTransform noteArea => _noteArea;
    public TextMeshProUGUI feedbackText => _feedbackText;
    public GameObject noteButtonPrefab => _noteButtonPrefab;
    public TextMeshProUGUI timerText => _timerText;

    private void Start()
    {
        spawnerService = new NoteSpawnerService();
        evaluatorService = new NoteEvaluatorService();

        // N�o iniciar modo automaticamente
        // SetMode(RhythmGameMode.CreateMode(selectedMode, this));
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
        modeStarted = false; // resetar flag ao trocar de modo
    }

    public void BeginMinigame()
    {
        if (currentMode == null)
        {
            SetMode(RhythmGameMode.CreateMode(selectedMode, this));
        }
        // Come�ar o modo explicitamente (chamar StartMode se existir)
        currentMode?.StartMode();
        modeStarted = true;
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

    public List<RhythmNote> GetActiveNotes() => activeNotes;

    // Implementa��o da interface IMinigame
    public void StartMinigame()
    {
        // S� configura o modo, mas n�o inicia
        SetMode(RhythmGameMode.CreateMode(selectedMode, this));
        // Quem chamar StartMinigame deve chamar BeginMinigame para come�ar
    }

    public void UpdateMinigame()
    {
        // Voc� pode usar Update normalmente, que j� cuida do controle com modeStarted
        Update();
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
        // Exemplo b�sico (ajuste conforme sua l�gica)
        return true;
    }
}
