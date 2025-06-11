using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    private IMinigame currentMinigame;
    private List<GameObject> currentMinigameUI = new List<GameObject>();
    private GameObject currentMinigameController;
    public GameObject MiniGameCanvas;
    private bool isRunning;
    public bool gameFinish;
    public System.Action<bool> OnMinigameFinished;
    [SerializeField] private Transform uiParent;

    private void Awake()
    {
        if (MiniGameCanvas != null)
        {
            MiniGameCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MiniGameCanvas não foi atribuído.");
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
            gameFinish = false;
            EndMinigame();
        }
    }

    public void StartMinigameWithUI(GameObject[] uiPrefabs, GameObject minigameControllerPrefab, IDifficultData difficult, GameModes.Modes mode)
    {
        if (MiniGameCanvas != null)
        {
            MiniGameCanvas.SetActive(true);
            Debug.Log("MiniGameCanvas ativado.");
        }

        currentMinigameUI = new List<GameObject>();

        foreach (GameObject prefab in uiPrefabs)
        {
            GameObject uiInstance = Instantiate(prefab, uiParent, false);
            currentMinigameUI.Add(uiInstance);
        }

        currentMinigameController = Instantiate(minigameControllerPrefab);
        currentMinigame = currentMinigameController.GetComponent<IMinigame>();

        if (currentMinigame == null)
        {
            Debug.LogError("O prefab do controlador não possui um componente que implementa IMinigame.");
            foreach (var ui in currentMinigameUI) Destroy(ui);
            Destroy(currentMinigameController);
            return;
        }

        SetMiniGame(difficult, mode);

        // Envia a primeira UI para o RhythmGameController, se necessário
        RhythmGameController rhythmController = currentMinigameController.GetComponent<RhythmGameController>();
        if (rhythmController != null && currentMinigameUI.Count > 0)
        {
            rhythmController.AssignUI(currentMinigameUI.ToArray());
        }

        isRunning = true;
        gameFinish = false;
        currentMinigame.StartMinigame();

        if (InputManager.Instance != null)
            InputManager.Instance.SetState(InputState.Minigame);

    }

    public void SetMiniGame(IDifficultData difficult, GameModes.Modes mode)
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
            OnMinigameFinished?.Invoke(success);
        }

        foreach (var ui in currentMinigameUI)
        {
            if (ui != null) Destroy(ui);
        }
        currentMinigameUI.Clear();

        if (currentMinigameController != null)
            Destroy(currentMinigameController);

        currentMinigame = null;
        isRunning = false;

        if (MiniGameCanvas != null)
            MiniGameCanvas.SetActive(false);

        if (InputManager.Instance != null)
            InputManager.Instance.SetState(InputState.Gameplay);
    }
}
