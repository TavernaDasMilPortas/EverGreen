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
            controller.StartMinigame(); // configura o modo, mas não começa ainda
            Debug.Log("scri: Modo configurado, aguardando início do minigame.");
        }
        else
        {
            Debug.LogWarning("scri: RhythmGameController não encontrado na cena!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("scri: Espaço pressionado, tentando iniciar minigame...");
            if (controller != null)
            {
                controller.BeginMinigame();  // começa o jogo só quando apertar espaço
                Debug.Log("scri: Minigame iniciado via BeginMinigame.");
            }
            else
            {
                Debug.LogWarning("scri: Não é possível iniciar minigame, controller é nulo.");
            }
        }
    }
}
