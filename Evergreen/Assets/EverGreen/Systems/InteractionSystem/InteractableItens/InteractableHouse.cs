using UnityEngine;

public class InteractableHouse : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    [Header("GameConfig")]
    public MinigameConfig config;

    [Header("Materiais para substituição")]
    public Renderer targetRenderer;
    public Material originalMaterial;
    public Material successMaterial;

    // Implementação das propriedades da interface (nomes exatos conforme a interface)
    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    public void Interact()
    {
        if (RequiredItem == null || InventoryManager.Instance.HasItem(RequiredItem, RequiredItemQuantity))
        {
            Debug.Log("iniciando interaçao");
            PerformInteraction();


        }
        else
        {
            Debug.Log("Item necessário: " + RequiredItem.itemName);
        }
    }

    private void PerformInteraction()
    {
        Debug.Log(interactionMessage);

        if (config != null && config.isRewarded == false)
        {
            MinigameManager.Instance.OnMinigameFinished += HandleMinigameResult;

            MinigameManager.Instance.StartMinigameWithUI(
                config.uiPrefab,
                config.minigameControllerPrefab,
                config.difficultData as IDifficultData,
                config.selectedMode,
                config.Recompeca
            );
        }
        else if (config == null)
        {
            Debug.LogError("Nenhum IMinigame encontrado neste objeto!");
        }
    }

    private void HandleMinigameResult(bool success)
    {
        // Remover o callback para evitar chamadas duplicadas
        MinigameManager.Instance.OnMinigameFinished -= HandleMinigameResult;

        if (success)
        {
            Debug.Log("Minigame foi concluído com sucesso!");
            InventoryManager.Instance.RemoveItem(requiredItem, requiredItemQuantity);

            config.isRewarded = true;

            ReplaceMaterial();
        }
        else
        {
            Debug.Log("Minigame falhou ou foi perdido.");
        }
    }

    private void ReplaceMaterial()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("Renderer alvo não atribuído.");
            return;
        }

        if (originalMaterial == null || successMaterial == null)
        {
            Debug.LogWarning("Materiais não definidos para substituição.");
            return;
        }

        Material[] mats = targetRenderer.materials;
        bool replaced = false;

        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] == originalMaterial)
            {
                mats[i] = successMaterial;
                replaced = true;
                break; // Remove isso se quiser substituir todas as ocorrências
            }
        }

        if (replaced)
        {
            targetRenderer.materials = mats;
            Debug.Log("Material substituído com sucesso.");
        }
        else
        {
            Debug.LogWarning("Material original não encontrado no renderer especificado.");
        }
    }
}
