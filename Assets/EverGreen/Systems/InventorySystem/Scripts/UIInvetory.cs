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
    public TMP_Text highlightedLabelQuantity;

    [Header("Tamanho da grade")]
    public int rows = 4;
    public int columns = 5;

    public int currentIndex = 0;
    private List<GameObject> slotObjects = new List<GameObject>();
    private Item nullItem;

    private void Awake()
    {
        Instance = this;
        inventoryUI.SetActive(false);

        // Cria um item "vazio"
        nullItem = new Item
        {
            itemName = "null",
            description = "",
            icon = null
        };

        GenerateSlots();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            if (inventoryUI.activeSelf)
            {
                currentIndex = 0;
            }
            UpdateUI();
        }

        if (!inventoryUI.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) currentIndex++;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) currentIndex--;

        int maxIndex = rows * columns - 1;
        currentIndex = Mathf.Clamp(currentIndex, 0, maxIndex);

        Highlight(currentIndex);
    }

    private void GenerateSlots()
    {
        int totalSlots = rows * columns;



        for (int i = 0; i < totalSlots; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            slotObjects.Add(obj);

            // Preenche com item nulo
            InventoryManager.Instance.inventory.Add(new InventorySlot
            {
                item = nullItem,
                quantity = 0
            });
        }
    }

    public void UpdateUI()
    {
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
        var inventory = InventoryManager.Instance.inventory;

        if (index < 0 || index >= inventory.Count)
            return;

        var slot = inventory[index];

        // Se for item "vazio", limpa destaque
        if (slot.item == null || slot.item.itemName == "null")
        {
            highlightedIcon.sprite = null;
            highlightedIcon.enabled = false;
            highlightedName.text = "";
            highlightedDescription.text = "";
            highlightedQuantity.text = "";
            highlightedLabelQuantity.text = ""; // <- Limpa o rótulo
            return;
        }

        highlightedIcon.sprite = slot.item.icon;
        highlightedIcon.enabled = true;
        highlightedName.text = slot.item.itemName;
        highlightedDescription.text = slot.item.description;
        highlightedQuantity.text = $"{slot.quantity}";
        highlightedLabelQuantity.text = "Quantidade"; // <- Mostra o rótulo
    }
    public Item NullItem => nullItem;
}
