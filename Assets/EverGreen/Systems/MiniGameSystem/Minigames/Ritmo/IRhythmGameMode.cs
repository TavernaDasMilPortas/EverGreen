using UnityEngine;

public interface IRhythmGameMode
{
    void StartMode();
    void UpdateMode();
    void HandleInput(KeyCode key);
    bool IsModeFinished { get; }

    void Initialize(IRhythmGameController controller);  // <-- Adicione esta linha
}