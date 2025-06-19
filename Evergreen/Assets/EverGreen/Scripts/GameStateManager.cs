using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public InputState CurrentState { get; private set; } = InputState.Gameplay;
    public InputState PreviousState { get; private set; } = InputState.Gameplay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Define um novo estado, armazenando o anterior automaticamente.
    /// </summary>
    public void SetState(InputState newState)
    {
        if (newState != CurrentState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
            Debug.Log($"[GameStateManager] Estado alterado: {PreviousState} ➜ {CurrentState}");
        }
    }

    /// <summary>
    /// Restaura o estado anterior.
    /// </summary>
    public void RestorePreviousState()
    {
        InputState temp = CurrentState;
        CurrentState = PreviousState;
        PreviousState = temp;
        Debug.Log($"[GameStateManager] Estado restaurado: {CurrentState}");
    }

    /// <summary>
    /// Força o reset para o estado padrão de gameplay.
    /// </summary>
    public void ResetToGameplay()
    {
        PreviousState = CurrentState;
        CurrentState = InputState.Gameplay;
        Debug.Log("[GameStateManager] Resetado para Gameplay.");
    }
}
