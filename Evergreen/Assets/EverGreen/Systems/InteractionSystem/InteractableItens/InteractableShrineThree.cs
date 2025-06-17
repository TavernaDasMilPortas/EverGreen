using UnityEngine;

public class InteractableShrineThree : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    [Header("GameConfig")]
    public MinigameConfig config;

    // Implementação das propriedades da interface (nomes exatos conforme a interface)
    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    public void Interact()
    {
        if (RequiredItem == null || InventoryManager.Instance.HasItem(RequiredItem, RequiredItemQuantity))
        {
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
                config.selectedMode);
        }
        else if (config == null)
        {
            Debug.LogError("Nenhum IMinigame encontrado neste objeto!");
        }
    }

    private void HandleMinigameResult(bool success)
    { 
        MinigameManager.Instance.OnMinigameFinished -= HandleMinigameResult;

        if (success)
        {
            MidpointManager.Instance.SpawnBridgeFromTree(MidpointManager.Instance.playerTransform.position, MidpointManager.Instance.closestTree.treeObject.transform.position);
            config.isRewarded = true;
            InteractionHandler.SafeDestroy(this);
        }
        else
        {

        }
        ShrineProgressionManager manager = FindObjectOfType<ShrineProgressionManager>();
        if (manager != null)
        {
            manager.RegisterInteraction();
        }
        else
        {
            Debug.LogWarning("ShrineProgressionManager não encontrado na cena.");
        }


    }



}