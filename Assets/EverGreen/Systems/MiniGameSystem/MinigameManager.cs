using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    private IMinigame currentMinigame;
    private bool isRunning;

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

    public void StartMinigame(IMinigame minigame)
    {
        currentMinigame = minigame;
        isRunning = true;
        currentMinigame.StartMinigame();
        InputManager.Instance.SetState(InputState.Minigame);
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

        currentMinigame = null;
        isRunning = false;
        InputManager.Instance.SetState(InputState.Gameplay);
    }
}