using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableStree : MonoBehaviour, IInteractable
{
    [Header("Item necessário para interação (opcional)")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = "Interagiu com Interactable";

    [Header("Cena a ser carregada")]
    public string sceneToLoad;

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
        Debug.Log(InteractionMessage);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Nenhuma cena especificada para carregar!");
        }
    }
}
