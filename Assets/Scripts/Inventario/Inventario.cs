using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Inventario : MonoBehaviour
{
    public GameObject inventoryPanel;  // Panel de UI que contiene el GridLayoutGroup
    public GameObject slotPrefab;      // Prefab para los slots en el inventario
    public GameObject itemPrefab;      // Prefab para los items en el inventario
    private List<GameObject> slots = new List<GameObject>(); // Lista de slots en el inventario
    private bool[] slotIsOccupied; // Array para mantener el estado ocupado de cada slot

    //UI
    public TextMeshProUGUI textMeshProNombre;
    public TextMeshProUGUI textMeshProDescripcion;
    public GameObject playerHands;
    public Animator handsAnimator;
    private Item selectedItem;  // �tem actualmente seleccionado
    void Start()
    {
        InitializeSlots();
        
    }
    void Awake()
    {
        if (inventoryPanel != null)
        {
           
         
        }
        else
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

        // Primero desactivar todos los par�metros para evitar superposiciones
        handsAnimator.SetBool("ConPistola", false);
        handsAnimator.SetBool("ConEscopeta", false);
        handsAnimator.SetBool("Disparando", false);
        handsAnimator.SetBool("DisparandoEscopeta", false);
        if (item == null)
        {
            // Desactiva cualquier arma
            foreach (Transform child in playerHands.transform)
            {
                child.gameObject.SetActive(false);
            }
            return;
        }

        // Desactiva todos los �tems/objetos armas activos
        foreach (Transform child in playerHands.transform)
        {
            child.gameObject.SetActive(false);
        }

        // Activa la arma espec�fica basada en el tipo de �tem
        Transform weaponTransform = playerHands.transform.Find(item.itemName);
        if (weaponTransform != null)
        {
            weaponTransform.gameObject.SetActive(true);
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
    private void RemoveItemFromInventory(Item item)
    {

        // Encuentra y destruye el objeto UI del �tem en el inventario
        for (int i = 0; i < slots.Count; i++)
        {
            DraggableItem draggableItem = slots[i].GetComponentInChildren<DraggableItem>();
            if (draggableItem != null && draggableItem.item == item)
            {
                slotIsOccupied[i] = false;  // Marca el slot como no ocupado
                Destroy(draggableItem.gameObject);  // Destruye el objeto del �tem
                break;
            }
        }
    }
    public void ClearSlots(Item item, int startIndex)
    {
        int numColumns = 10; // Configuraci�n de columnas en el GridLayoutGroup
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
        if (selectedItem != null && selectedItem.itemType == ItemType.Consumable)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.Heal(selectedItem.healingAmount);  // Curar al jugador con la cantidad especificada
                RemoveItemFromInventory(selectedItem);   // Eliminar el �tem del inventario
                selectedItem = null;                     // Limpiar la selecci�n actual
                UpdateItemInfoUI(null);                  // Actualizar la UI para reflejar que no hay �tem seleccionado
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
            selectedItem = null; // Opcional: Limpiar la selecci�n despu�s de equipar
            UpdateItemInfoUI(null); // Actualizar la UI para reflejar que no hay �tem seleccionado
        }
        else
        {
            Debug.Log("No item selected to equip.");
        }
    }
    public void ItemSelected(Item item)
    {
        selectedItem = item;  // Actualiza el �tem seleccionado
        UpdateItemInfoUI(item);  // Actualiza la UI con la informaci�n del �tem
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
            textMeshProNombre.text = "Selecciona un �tem";
            textMeshProDescripcion.text = "";
        }
    }
  
    public void InitializeSlots()
    {
        // Asigna un n�mero fijo de slots, por ejemplo 24 slots (6x4 grid)
        int numSlots = 60;
        slots.Clear();
        slotIsOccupied = new bool[numSlots]; // Inicializa el array de ocupaci�n

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
            if (!slotIsOccupied[i] && CheckIfFits(item, i)) // Aseg�rate de verificar si el slot est� ocupado
            {
                PlaceItemInSlots(item, i);
                return;
            }
        }
        Debug.Log("No hay suficiente espacio en el inventario para este item");
    }
   

    private bool CheckIfFits(Item item, int startIndex)
    {
        int numColumns = 10; // Asume que el inventario es de 6 slots de ancho
        int numRows = slots.Count / numColumns; // Asume una cantidad fija de filas basado en el total de slots
        int row = startIndex / numColumns;
        int column = startIndex % numColumns;

        // Verifica si el item se sale de los l�mites del inventario
        if (column + item.size.x > numColumns || row + item.size.y > numRows)
            return false;

        // Verifica si alg�n slot necesario est� ya ocupado
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
        int numColumns = 10; // Configuraci�n de columnas en el GridLayoutGroup
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
            int numColumns = 10; // Configuraci�n de columnas en el GridLayoutGroup
            GameObject initialSlot = slots[startIndex];
            GameObject inventoryParent = inventoryPanel.transform.parent.gameObject;  // Parent deseado para los items

            // Instancia el item prefab en el inventario
            GameObject itemUI = Instantiate(itemPrefab, inventoryParent.transform);
            DraggableItem draggableComponent = itemUI.GetComponent<DraggableItem>();

            // Configura el item usando el script DraggableItem, pasando tambi�n el parent original
            if (draggableComponent != null)
            {
                draggableComponent.SetupItem(item, startIndex, inventoryParent.transform);  // Pasando el Transform del parent deseado
            }
            else
            {
                Debug.LogError("DraggableItem component not found on item prefab!");
                return;  // Detiene la ejecuci�n si no se encuentra el componente
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

            // Marcar slots como ocupados
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
}