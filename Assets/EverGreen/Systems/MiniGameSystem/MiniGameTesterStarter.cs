using UnityEngine;

public class MinigameTestStarter : MonoBehaviour
{
    [Header("Prefabs do Minigame")]
    public GameObject uiPrefab; // Prefab da UI do minigame
    public GameObject minigameControllerPrefab; // Prefab do controlador do minigame
    public ScriptableObject difficultData; // Dados de dificuldade do minigame
    public GameModes.Modes selectedMode; // Modo de jogo selecionado
    [Header("Hierarquia da UI")]
    public Transform uiParent; // Painel ou Canvas onde a UI será instanciada

    private bool minigameStarted = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !minigameStarted)
        {
            StartMinigame();
            minigameStarted = true;
        }
    }

    void StartMinigame()
    {
        MinigameManager.Instance.StartMinigameWithUI(uiPrefab, minigameControllerPrefab, difficultData as IDifficultData, selectedMode);
    }
}
