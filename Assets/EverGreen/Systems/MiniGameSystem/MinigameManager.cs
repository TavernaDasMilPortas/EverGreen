using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }
    private IMinigame currentMinigame;
    private GameObject currentMinigameUI;
    private GameObject currentMinigameController;
    private bool isRunning;
    [SerializeField] private Transform uiParent;

    private void Awake()
    {
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
    }

    /// <summary>
    /// Inicia o minigame instanciando a UI e o controlador.
    /// </summary>
    /// <param name="uiPrefab">Prefab da UI do minigame.</param>
    /// <param name="minigameControllerPrefab">Prefab do controlador do minigame (com um script que implementa IMinigame).</param>
    public void StartMinigameWithUI(GameObject uiPrefab, GameObject minigameControllerPrefab, IDifficultData difficult, GameModes.Modes mode)
    {
        // Instancia a UI como filha do painel
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
        isRunning = true;
        currentMinigame.StartMinigame();
        InputManager.Instance.SetState(InputState.Minigame);
    }
    public void SetMiniGame(IDifficultData difficult , GameModes.Modes mode)
    {
        Debug.Log("Definindo minigame com dificuldade: " + difficult + " e modo: " + mode);
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

        // Destroi os objetos instanciados
        if (currentMinigameUI != null) Destroy(currentMinigameUI);
        if (currentMinigameController != null) Destroy(currentMinigameController);

        currentMinigame = null;
        isRunning = false;
        InputManager.Instance.SetState(InputState.Gameplay);
    }
}
