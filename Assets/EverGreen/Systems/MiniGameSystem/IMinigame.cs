// Interface padronizada para qualquer minigame
using UnityEngine;

public interface IMinigame
{

    void StartMinigame();
    void UpdateMinigame();
    void HandleInput(KeyCode key);
    void EndMinigame();
    bool EvaluateResult();
    void AssignUI(GameObject uiRoot);
    void SetDifficulty(IDifficultData difficulty);
    void SelectMode(GameModes.Modes mode);
    //GameObject gamePrefab { get; }
}