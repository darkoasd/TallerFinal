using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Inventario : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    private List<GameObject> slots = new List<GameObject>();
    private bool[] slotIsOccupied;

    public TextMeshProUGUI textMeshProNombre;
    public TextMeshProUGUI textMeshProDescripcion;
    public GameObject playerHands;
    public Animator handsAnimator;
    private Item selectedItem;
    private Item currentEquippedWeapon;  // Variable para guardar el arma equipada actual

    void Start()
    {
        InitializeSlots();
    }

    void Awake()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory panel is not assigned!");
        }
    }

    public void EquipItem(Item item)
    {
        if (handsAnimator == null)
        {
            Debug.LogError("Animator component not assigned or found!");
            return;
        }

        handsAnimator.SetBool("ConPistola", false);
        handsAnimator.SetBool("ConEscopeta", false);
        handsAnimator.SetBool("Disparando", false);
        handsAnimator.SetBool("DisparandoEscopeta", false);

        if (item == null)
        {
            foreach (Transform child in playerHands.transform)
            {
                child.gameObject.SetActive(false);
            }
            currentEquippedWeapon = null;  // Desactivar el arma equipada actual
            return;
        }

        foreach (Transform child in playerHands.transform)
        {
            child.gameObject.SetActive(false);
        }

        Transform weaponTransform = playerHands.transform.Find(item.itemName);
        if (weaponTransform != null)
        {
            weaponTransform.gameObject.SetActive(true);
            currentEquippedWeapon = item;  // Actualizar el arma equipada actual
            if (item.itemType == ItemType.Weapon)
            {
                switch (item.itemName)
                {
                    case "Pistola":
                        handsAnimator.SetBool("ConPistola", true);
                        break;
                    case "Escopeta":
                        handsAnimator.SetBool("ConEscopeta", true);
                        break;
                }
            }
        }
        else
        {
            Debug.LogError("Assigned weapon not found in playerHands children.");
        }
    }

    public void RemoveItemFromInventory(Item item, int quantity)
    {
        int count = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            DraggableItem draggableItem = slots[i].GetComponentInChildren<DraggableItem>();
            if (draggableItem != null && draggableItem.item == item)
            {
                slotIsOccupied[i] = false;
                Destroy(draggableItem.gameObject);
                count++;
                if (count >= quantity)
                    return;
            }
        }

        DraggableItem[] draggableItems = GetComponentsInChildren<DraggableItem>();
        foreach (var draggableItem in draggableItems)
        {
            if (draggableItem.item == item)
            {
                Destroy(draggableItem.gameObject);
                count++;
                if (count >= quantity)
                    return;
            }
        }
    }

    public void ClearSlots(Item item, int startIndex)
    {
        int numColumns = 10;
        for (int y = 0; y < item.size.y; y++)
        {
            for (int x = 0; x < item.size.x; x++)
            {
                int slotIndex = startIndex + x + y * numColumns;
                if (slotIndex < slots.Count)
                {
                    slotIsOccupied[slotIndex] = false;
                }
            }
        }
    }

    public void UseSelectedItem()
    {
        if (selectedItem != null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                selectedItem.ApplyEffect(player);
                RemoveItemFromInventory(selectedItem, 1);
                selectedItem = null;
                UpdateItemInfoUI(null);
            }
        }
    }

    private void ApplyConsumableEffect(Item item)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.Heal(item.healingAmount);
            Debug.Log("Consumible usado: " + item.itemName);
        }
    }

    public void TryEquipSelectedItem()
    {
        if (selectedItem != null)
        {
            EquipItem(selectedItem);
            selectedItem = null;
            UpdateItemInfoUI(null);
        }
        else
        {
            Debug.Log("No item selected to equip.");
        }
    }

    public void ItemSelected(Item item)
    {
        selectedItem = item;
        UpdateItemInfoUI(item);
    }

    public void UpdateItemInfoUI(Item item)
    {
        if (item != null)
        {
            textMeshProNombre.text = item.itemName;
            textMeshProDescripcion.text = item.description;
        }
        else
        {
            textMeshProNombre.text = "Selecciona un ítem";
            textMeshProDescripcion.text = "";
        }
    }

    public void InitializeSlots()
    {
        int numSlots = 60;
        slots.Clear();
        slotIsOccupied = new bool[numSlots];

        for (int i = 0; i < numSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel.transform);
            slots.Add(slot);
            slotIsOccupied[i] = false;
        }
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slotIsOccupied[i] && CheckIfFits(item, i))
            {
                PlaceItemInSlots(item, i);
                Debug.Log("Item added: " + item.itemName);
                ReequipCurrentWeapon();  // Reequipar el arma actual después de añadir un ítem
                return;
            }
        }
        Debug.Log("No hay suficiente espacio en el inventario para este item");
    }

    private bool CheckIfFits(Item item, int startIndex)
    {
        int numColumns = 10;
        int numRows = slots.Count / numColumns;
        int row = startIndex / numColumns;
        int column = startIndex % numColumns;

        if (column + item.size.x > numColumns || row + item.size.y > numRows)
            return false;

        for (int y = 0; y < item.size.y; y++)
        {
            for (int x = 0; x < item.size.x; x++)
            {
                int slotIndex = startIndex + x + y * numColumns;
                if (slotIndex >= slots.Count || slotIsOccupied[slotIndex])
                    return false;
            }
        }
        return true;
    }

    public void MarkSlotsAsOccupied(Item item, int startIndex)
    {
        int numColumns = 10;
        for (int y = 0; y < item.size.y; y++)
        {
            for (int x = 0; x < item.size.x; x++)
            {
                int slotIndex = startIndex + x + y * numColumns;
                if (slotIndex < slots.Count)
                {
                    slotIsOccupied[slotIndex] = true;
                }
            }
        }
    }

    private void PlaceItemInSlots(Item item, int startIndex)
    {
        Debug.Log("Placing item: " + item.itemName);
        if (startIndex >= 0 && startIndex < slots.Count)
        {
            int numColumns = 10;
            GameObject initialSlot = slots[startIndex];
            GameObject inventoryParent = inventoryPanel.transform.parent.gameObject;

            GameObject itemUI = Instantiate(itemPrefab, inventoryParent.transform);
            DraggableItem draggableComponent = itemUI.GetComponent<DraggableItem>();

            if (draggableComponent != null)
            {
                draggableComponent.SetupItem(item, startIndex, inventoryParent.transform);
            }
            else
            {
                Debug.LogError("DraggableItem component not found on item prefab!");
                return;
            }

            RectTransform itemRect = itemUI.GetComponent<RectTransform>();
            GridLayoutGroup gridLayout = inventoryPanel.GetComponent<GridLayoutGroup>();
            Vector2 cellSize = gridLayout.cellSize;
            Vector2 spacing = gridLayout.spacing;

            int baseRow = startIndex / numColumns;
            int baseCol = startIndex % numColumns;

            RectTransform slotRect = initialSlot.GetComponent<RectTransform>();
            float posX = slotRect.anchoredPosition.x + ((cellSize.x + spacing.x) * (item.size.x - 1)) / 2;
            float posY = slotRect.anchoredPosition.y - ((cellSize.y + spacing.y) * (item.size.y - 1)) / 2;

            itemRect.anchoredPosition = new Vector2(posX, posY);
            itemRect.sizeDelta = new Vector2(cellSize.x * item.size.x + spacing.x * (item.size.x - 1), cellSize.y * item.size.y + spacing.y * (item.size.y - 1));

            MarkSlotsAsOccupied(item, startIndex);
        }
    }

    public bool TryGetPositionForItem(Item item, RectTransform itemRect, Vector2 position, out Vector2 newPos, out int newSlotIndex)
    {
        newPos = new Vector2();
        newSlotIndex = -1;

        GridLayoutGroup gridLayout = inventoryPanel.GetComponent<GridLayoutGroup>();
        Vector2 cellSize = gridLayout.cellSize;
        Vector2 spacing = gridLayout.spacing;
        int numColumns = 10;

        for (int i = 0; i < slots.Count; i++)
        {
            RectTransform slotRect = slots[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, position, null))
            {
                bool fits = true;
                for (int y = 0; y < item.size.y; y++)
                {
                    for (int x = 0; x < item.size.x; x++)
                    {
                        int slotIndex = i + x + y * numColumns;
                        if (slotIndex >= slots.Count || slotIsOccupied[slotIndex])
                        {
                            fits = false;
                            break;
                        }
                    }
                    if (!fits) break;
                }

                if (fits)
                {
                    newPos.x = slotRect.anchoredPosition.x + ((cellSize.x + spacing.x) * (item.size.x - 1)) / 2;
                    newPos.y = slotRect.anchoredPosition.y - ((cellSize.y + spacing.y) * (item.size.y - 1)) / 2;
                    newSlotIndex = i;
                    return true;
                }
            }
        }
        return false;
    }

    public bool HasItem(string itemName, int quantity)
    {
        int count = 0;
        foreach (var slot in slots)
        {
            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>(true);
            if (draggableItem != null && draggableItem.item.itemName == itemName)
            {
                count++;
                Debug.Log($"Found {count} of {itemName}");
                if (count >= quantity) return true;
            }
        }

        DraggableItem[] draggableItems = GetComponentsInChildren<DraggableItem>(true);
        foreach (var draggableItem in draggableItems)
        {
            if (draggableItem.item.itemName == itemName)
            {
                count++;
                Debug.Log($"Found {count} of {itemName} in external items");
                if (count >= quantity) return true;
            }
        }
        Debug.Log($"Only found {count} of {itemName}, needed {quantity}");
        return false;
    }

    public int GetItemCount(string itemName)
    {
        int count = 0;
        foreach (var slot in slots)
        {
            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
            if (draggableItem != null && draggableItem.item.itemName == itemName)
            {
                count++;
            }
        }
        return count;
    }

    private void ReequipCurrentWeapon()
    {
        if (currentEquippedWeapon != null)
        {
            EquipItem(currentEquippedWeapon);
            // Asegúrate de que el script del arma esté habilitado
            Pistola pistola = playerHands.transform.Find(currentEquippedWeapon.itemName)?.GetComponent<Pistola>();
            if (pistola != null)
            {
                pistola.enabled = false; // Deshabilitar temporalmente
                pistola.enabled = true;  // Volver a habilitar
                pistola.ActualizarTextoMunicion(); // Actualizar la UI de munición
            }
        }
    }
}