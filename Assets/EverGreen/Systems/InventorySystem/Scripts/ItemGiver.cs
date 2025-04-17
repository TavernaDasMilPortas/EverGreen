using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [Header("Item a ser dado ao invent�rio")]
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
            Debug.LogWarning("Nenhum item foi atribu�do para dar ao invent�rio.");
            return;
        }

        InventoryManager.Instance.AddItem(itemParaDar, quantidade);
        Debug.Log($"Item '{itemParaDar.itemName}' adicionado ao invent�rio (x{quantidade}).");
    }
}