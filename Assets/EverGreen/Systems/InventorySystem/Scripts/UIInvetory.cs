using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIInventory : UIMenuTab
{
    public GameObject inventoryUI;
    public Transform slotContainer;
    public GameObject slotPrefab;

    public Image highlightedIcon;
    public TMP_Text highlightedName;
    public TMP_Text highlightedDescription;
    public TMP_Text highlightedQuantity;
    public TMP_Text highlightedLabelQuantity;

    [Header("Tamanho da grade")]
    public int rows = 4;
    public int columns = 5;

    private int currentIndex = 0;
    private List<GameObject> slotObjects = new List<GameObject>();
    private Item nullItem;

    public static UIInventory Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        inventoryUI.SetActive(false);

        nullItem = new Item
        {
            itemName = "null",
            description = "",
            icon = null
        };

        GenerateSlots();
    }

    private void GenerateSlots()
    {
        int totalSlots = rows * columns;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            slotObjects.Add(obj);

            InventoryManager.Instance.inventory.Add(new InventorySlot
            {
                item = nullItem,
                quantity = 0
            });
        }
    }

    public override void Toggle(bool open)
    {
        Debug.Log("UIInventory.Toggle chamado com open = " + open);

        if (InputManager.Instance.CurrentState == InputState.Minigame)
        {
            Debug.Log("Toggle cancelado: estado é Minigame.");
            return;
        }

        inventoryUI.SetActive(open);
        IsOpen = open;

        if (open)
        {
            Debug.Log("Menu do inventário aberto.");
            currentIndex = 0;
            UpdateUI();
        }
        else
        {
            Debug.Log("Menu do inventário fechado.");
        }
    }

    public override void Navigate(Vector2 direction)
    {
        if (!IsOpen)
        {
            Debug.Log("Navigate cancelado: menu está fechado.");
            return;
        }

        Debug.Log("Navigate chamado com direção: " + direction);

        int newIndex = currentIndex;

        if (direction == Vector2.right) newIndex += 1;
        else if (direction == Vector2.left) newIndex -= 1;
        else if (direction == Vector2.down) newIndex += columns;
        else if (direction == Vector2.up) newIndex -= columns;

        int maxIndex = rows * columns - 1;
        newIndex = Mathf.Clamp(newIndex, 0, maxIndex);

        if (newIndex != currentIndex)
        {
            Debug.Log($"Índice alterado de {currentIndex} para {newIndex}");
            currentIndex = newIndex;
            Highlight(currentIndex);
        }
        else
        {
            Debug.Log("Índice permaneceu o mesmo: " + currentIndex);
        }
    }

    public override void Confirm()
    {
        Debug.Log("UIInventory.Confirm chamado");
        // Implementar ações se necessário
    }

    public override void Cancel()
    {
        Debug.Log("UIInventory.Cancel chamado");
        Toggle(false);
        InputManager.Instance.SetState(InputState.Gameplay);
    }

    public void UpdateUI()
    {
        Debug.Log("UpdateUI chamado");

        var inventory = InventoryManager.Instance.inventory;

        for (int i = 0; i < slotObjects.Count; i++)
        {
            Image icon = slotObjects[i].transform.Find("Icon").GetComponent<Image>();

            if (inventory[i].item != null && inventory[i].item.itemName != "null")
            {
                icon.sprite = inventory[i].item.icon;
                icon.enabled = true;
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
            }
        }

        Highlight(currentIndex);
    }

    private void Highlight(int index)
    {
        Debug.Log("Highlight chamado com index = " + index);

        var inventory = InventoryManager.Instance.inventory;

        if (index < 0 || index >= inventory.Count)
        {
            Debug.LogWarning("Highlight cancelado: índice fora do intervalo.");
            return;
        }

        var slot = inventory[index];

        if (slot.item == null || slot.item.itemName == "null")
        {
            Debug.Log("Slot vazio no índice " + index);
            highlightedIcon.sprite = null;
            highlightedIcon.enabled = false;
            highlightedName.text = "";
            highlightedDescription.text = "";
            highlightedQuantity.text = "";
            highlightedLabelQuantity.text = "";
            return;
        }

        Debug.Log("Slot com item encontrado: " + slot.item.itemName);

        highlightedIcon.sprite = slot.item.icon;
        highlightedIcon.enabled = true;
        highlightedName.text = slot.item.itemName;
        highlightedDescription.text = slot.item.description;
        highlightedQuantity.text = $"{slot.quantity}";
        highlightedLabelQuantity.text = "Quantidade";
    }


    public Item NullItem => nullItem;
}
