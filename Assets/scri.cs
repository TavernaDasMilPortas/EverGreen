using UnityEngine;

public class scri : MonoBehaviour
{
    private RhythmGameController controller;

    void Start()
    {
        Debug.Log("scri: Start chamado, procurando RhythmGameController...");
        controller = FindObjectOfType<RhythmGameController>();

        if (controller != null)
        {
            Debug.Log("scri: RhythmGameController encontrado, definindo modo Classic e configurando minigame.");
            controller.selectedMode = RhythmGameMode.RhythmGameModeType.Piano;
            controller.StartMinigame(); // configura o modo, mas n�o come�a ainda
            Debug.Log("scri: Modo configurado, aguardando in�cio do minigame.");
        }
        else
        {
            Debug.LogWarning("scri: RhythmGameController n�o encontrado na cena!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("scri: Espa�o pressionado, tentando iniciar minigame...");
            if (controller != null)
            {
                controller.BeginMinigame();  // come�a o jogo s� quando apertar espa�o
                Debug.Log("scri: Minigame iniciado via BeginMinigame.");
            }
            else
            {
                Debug.LogWarning("scri: N�o � poss�vel iniciar minigame, controller � nulo.");
            }
        }
    }
}
