using UnityEngine;
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;

    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    // Construtor sem parâmetros (obrigatório para usar new InventorySlot())
    public InventorySlot() { }
}