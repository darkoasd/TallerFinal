using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; // Panel que contiene la cuadrícula del inventario
    public InventorySlotsUI[,] slotsUI;  // Matriz de componentes UI para los slots
    public GameObject slotPrefab; // Prefab para los slots

    public static InventoryUI instance;

    public int selectedX, selectedY;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of InventoryUI found!");
            return;
        }
        instance = this;
        InitializeInventoryGrid();
    }

    void Start()
    {
        InitializeInventoryGrid();
    }

    void InitializeInventoryGrid()
    {
        int width = Inventory.instance.width;
        int height = Inventory.instance.height;
        slotsUI = new InventorySlotsUI[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject slotObj = Instantiate(slotPrefab, inventoryPanel.transform);
                InventorySlotsUI slotUI = slotObj.GetComponent<InventorySlotsUI>();
                if (slotUI != null)
                {
                    slotUI.x = x;
                    slotUI.y = y;
                    slotsUI[x, y] = slotUI;
                }
                else
                {
                    Debug.LogError("The slot prefab does not have an InventorySlotUI component attached.");
                }
            }
        }
    }

    public void UpdateInventoryUI()
    {
        for (int x = 0; x < Inventory.instance.width; x++)
        {
            for (int y = 0; y < Inventory.instance.height; y++)
            {
                ItemSlots slot = Inventory.instance.GetSlot(x, y);
                InventorySlotsUI slotUI = slotsUI[x, y];
                Debug.Log($"Updating slot {x}, {y} - Occupied: {slot.IsOccupied}");

                if (slot.IsOccupied)
                {
                    Debug.Log($"Item: {slot.Item.itemName}");
                    slotUI.SetItem(slot.Item);
                }
                else
                {
                    slotUI.ClearSlot();
                }
            }
        }
    }

    public void ShowPickup(Item item)
    {
        // Podrías necesitar lógica aquí para manejar la visualización de la UI
        // que permite al jugador elegir dónde colocar el item.
    }

    public void HidePickup()
    {
        // Ocultar información del ítem
    }
    public void SelectSlot(int x, int y)
    {
        selectedX = x;
        selectedY = y;
        // Aquí puedes añadir lógica para visualizar la selección al usuario,
        // como cambiar el color del fondo del slot o mostrar un borde alrededor del slot seleccionado.
    }
}
