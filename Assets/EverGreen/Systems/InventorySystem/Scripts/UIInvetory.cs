using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance;

    public GameObject inventoryUI;
    public Transform slotContainer;
    public GameObject slotPrefab;

    public Image highlightedIcon;
    public TMP_Text highlightedName;
    public TMP_Text highlightedDescription;
    public TMP_Text highlightedQuantity;

    private int currentIndex = 0;

    private void Awake()
    {
        Instance = this;
        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            UpdateUI();
        }

        if (!inventoryUI.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) currentIndex++;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) currentIndex--;
        currentIndex = Mathf.Clamp(currentIndex, 0, InventoryManager.Instance.inventory.Count - 1);

        Highlight(currentIndex);
    }

    public void UpdateUI()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (var slot in InventoryManager.Instance.inventory)
        {
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            obj.GetComponent<Image>().sprite = slot.item.icon;
        }

        Highlight(currentIndex);
    }

    private void Highlight(int index)
    {
        if (InventoryManager.Instance.inventory.Count == 0) return;

        var slot = InventoryManager.Instance.inventory[index];
        highlightedIcon.sprite = slot.item.icon;
        highlightedName.text = slot.item.itemName;
        highlightedDescription.text = slot.item.description;
        highlightedQuantity.text = $"Quantidade: {slot.quantity}";
    }
}