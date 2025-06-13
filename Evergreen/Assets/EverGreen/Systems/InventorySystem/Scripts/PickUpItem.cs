using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;
    public int quantity = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(item, quantity);
            Destroy(gameObject);
        }
    }
}