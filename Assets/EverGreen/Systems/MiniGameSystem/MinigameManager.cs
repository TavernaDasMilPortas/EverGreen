using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }
    private IMinigame currentMinigame;
    private GameObject currentMinigameUI;
    private GameObject currentMinigameController;
    public GameObject MiniGameCanvas;
    private bool isRunning;
    public bool gameFinish;

    [SerializeField] private Transform uiParent; // Painel onde a UI do minigame será instanciada

    private void Awake()
    {
        // Tenta encontrar o GameObject com o nome "MiniGameCanvas" na cena

        if (MiniGameCanvas != null)
        {
            MiniGameCanvas.SetActive(false); // Desativa ao iniciar o jogo
        }
        else
        {
            Debug.LogWarning("MiniGameCanvas não encontrado na cena.");
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isRunning && currentMinigame != null)
        {
            currentMinigame.UpdateMinigame();
        }

        if (gameFinish)
        {
            gameFinish = false; // Garante que não chamará múltiplas vezes
            EndMinigame();
        }
    }

    public void StartMinigameWithUI(GameObject uiPrefab, GameObject minigameControllerPrefab, IDifficultData difficult, GameModes.Modes mode)
    {
        if (MiniGameCanvas != null)
        {
            MiniGameCanvas.SetActive(true); // Ativa o Canvas ao iniciar o minigame
            Debug.Log("MiniGameCanvas ativado");
        }
        currentMinigameUI = Instantiate(uiPrefab, uiParent, false);
        currentMinigameController = Instantiate(minigameControllerPrefab);
        currentMinigame = currentMinigameController.GetComponent<IMinigame>();

        if (currentMinigame == null)
        {
            Debug.LogError("O prefab do controlador não possui um componente que implementa IMinigame.");
            Destroy(currentMinigameUI);
            Destroy(currentMinigameController);
            return;
        }

        SetMiniGame(difficult, mode);

        RhythmGameController rhythmController = currentMinigameController.GetComponent<RhythmGameController>();
        if (rhythmController != null)
        {
            rhythmController.AssignUI(currentMinigameUI);
        }

        isRunning = true;
        gameFinish = false;
        currentMinigame.StartMinigame();

        if (InputManager.Instance != null)
            InputManager.Instance.SetState(InputState.Minigame);
    }

    public void SetMiniGame(IDifficultData difficult, GameModes.Modes mode)
    {
        currentMinigame?.SetDifficulty(difficult);
        currentMinigame?.SelectMode(mode);
    }

    public void HandleInput(KeyCode key)
    {
        if (isRunning && currentMinigame != null)
        {
            currentMinigame.HandleInput(key);
        }
    }

    public void EndMinigame()
    {
        if (currentMinigame != null)
        {
            currentMinigame.EndMinigame();
            bool success = currentMinigame.EvaluateResult();
            Debug.Log("Minigame finalizado. Sucesso: " + success);
        }

        if (currentMinigameUI != null) Destroy(currentMinigameUI);
        if (currentMinigameController != null) Destroy(currentMinigameController);

        currentMinigame = null;
        isRunning = false;

        if (MiniGameCanvas != null)
            MiniGameCanvas.SetActive(false); // Desativa o Canvas ao terminar o minigame

        if (InputManager.Instance != null)
            InputManager.Instance.SetState(InputState.Gameplay);
    }
}
