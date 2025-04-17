using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [Header("Item a ser dado ao inventário")]
    public Item itemParaDar;
    public int quantidade = 1;

    [Header("Tecla para entregar o item")]
    public KeyCode teclaParaDar = KeyCode.T;

    private void Update()
    {
        if (Input.GetKeyDown(teclaParaDar))
        {
            DarItem();
        }
    }

    public void DarItem()
    {
        if (itemParaDar == null)
        {
            Debug.LogWarning("Nenhum item foi atribuído para dar ao inventário.");
            return;
        }

        InventoryManager.Instance.AddItem(itemParaDar, quantidade);
        Debug.Log($"Item '{itemParaDar.itemName}' adicionado ao inventário (x{quantidade}).");
    }
}