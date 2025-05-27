using UnityEngine;

public interface InteractableObject : IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem; // pode ser null
    public int requiredItemQuantity; // pode ser null


    public void Interact()
    {
        if (requiredItem == null || InventoryManager.Instance.HasItem(requiredItem))
        {
            PerformInteraction();
        }

    }

    private void PerformInteraction()
    {
        Debug.Log(interactionMessage);
        // Aqui vai a lógica de interação específica: abrir porta, pegar item, etc.
    }
}
