using UnityEngine;

public class MinigameConfig : MonoBehaviour
{
    public GameObject[] uiPrefab; // Prefab da UI do minigame
    public GameObject minigameControllerPrefab; // Prefab do controlador do minigame
    public ScriptableObject difficultData; // Dados de dificuldade do minigame
    public GameModes.Modes selectedMode; // Modo de jogo selecionado
    public Item Recompeca;
    public bool isRewarded = false;
}
