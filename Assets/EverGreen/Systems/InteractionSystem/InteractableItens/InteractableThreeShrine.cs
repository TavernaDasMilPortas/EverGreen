using UnityEngine;

public class InteractableThreeShrine : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

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

        IMinigame minigame = GetComponent<IMinigame>();
        if (minigame != null)
        {
            MinigameManager.Instance.StartMinigame(minigame);

            // Após StartMinigame, já chama o início real do minigame:
            minigame.StartMinigame();

            // Se o minigame tiver etapas adicionais, o próprio StartMinigame deve cuidar disso.
            // Por exemplo, RhythmGameController.SetMode + BeginMinigame.
        }
        else
        {
            Debug.LogError("Nenhum IMinigame encontrado neste objeto!");
        }
    }

}