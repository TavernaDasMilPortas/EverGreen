using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> inventory = new List<InventorySlot>();
    public int maxSlots = 20;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddItem(Item item, int quantity = 1)
    {
        InventorySlot slot = inventory.Find(i => i.item == item);
        if (slot != null)
        {
            slot.quantity += quantity;
        }
        else if (inventory.Count < maxSlots)
        {
            inventory.Add(new InventorySlot(item, quantity));
        }
        else
        {
            Debug.Log("Inventário cheio!");
        }

        UIInventory.Instance.UpdateUI();
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        InventorySlot slot = inventory.Find(i => i.item == item);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0)
                inventory.Remove(slot);
        }

        UIInventory.Instance.UpdateUI();
    }
}
