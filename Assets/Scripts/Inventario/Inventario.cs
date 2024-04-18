using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    public GameObject inventoryPanel;  // Panel de UI que contiene el GridLayoutGroup
    public GameObject slotPrefab;      // Prefab para los slots en el inventario
    public GameObject itemPrefab;      // Prefab para los items en el inventario
    private List<GameObject> slots = new List<GameObject>(); // Lista de slots en el inventario
    private bool[] slotIsOccupied; // Array para mantener el estado ocupado de cada slot

    void Start()
    {
        InitializeSlots();
    }

   
    public void InitializeSlots()
    {
        // Asigna un número fijo de slots, por ejemplo 24 slots (6x4 grid)
        int numSlots = 24;
        slots.Clear();
        slotIsOccupied = new bool[numSlots]; // Inicializa el array de ocupación

        for (int i = 0; i < numSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel.transform);
            slots.Add(slot);
            slotIsOccupied[i] = false; // Todos los slots comienzan como no ocupados
        }
    }
    public void AddItem(Item item)
    {
        // Intentar encontrar un lugar en el inventario donde el item pueda ser colocado
        for (int i = 0; i < slots.Count; i++)
        {
            if (CheckIfFits(item, i))
            {
                PlaceItemInSlots(item, i);
                MarkSlotsAsOccupied(item, i);
                return;
            }
        }
        Debug.Log("No hay suficiente espacio en el inventario para este item");
    }

    private bool CheckIfFits(Item item, int startIndex)
    {
        int numColumns = 6; // Asume que el inventario es de 6 slots de ancho
        int row = startIndex / numColumns;
        int column = startIndex % numColumns;

        if (column + item.size.x > numColumns || row + item.size.y > slots.Count / numColumns)
            return false;

        for (int y = 0; y < item.size.y; y++)
        {
            for (int x = 0; x < item.size.x; x++)
            {
                int slotIndex = startIndex + x + y * numColumns;
                if (slotIndex >= slots.Count || slotIsOccupied[slotIndex])
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void MarkSlotsAsOccupied(Item item, int startIndex)
    {
        int numColumns = 6; // Configuración de columnas en el GridLayoutGroup
        for (int y = 0; y < item.size.y; y++)
        {
            for (int x = 0; x < item.size.x; x++)
            {
                int slotIndex = startIndex + x + y * numColumns;
                if (slotIndex < slotIsOccupied.Length)
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
            int numColumns = 6; // Configuración de columnas en el GridLayoutGroup
            GameObject initialSlot = slots[startIndex];
            GameObject inventoryParent = inventoryPanel.transform.parent.gameObject;

            GameObject itemUI = Instantiate(itemPrefab, inventoryParent.transform);
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

            Image itemImage = itemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = item.icon;
                itemImage.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                Debug.LogError("Image component not found on item prefab!");
            }
            // Asegúrate de que el TextMeshPro componente se actualiza correctamente
            TMPro.TextMeshProUGUI textMesh = itemUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = item.itemName;
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on item prefab!");
            }

            // Marcar slots como ocupados
            MarkSlotsAsOccupied(item, startIndex);
        }
    }
}