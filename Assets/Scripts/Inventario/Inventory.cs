using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int width = 10;
    public int height = 6;
    public ItemSlots[,] slots;
    public static Inventory instance; 
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
        InitializeSlots();
    }
    private void InitializeSlots()
    {
        slots = new ItemSlots[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                slots[x, y] = new ItemSlots(); // Asegúrate de que ItemSlot pueda manejar estados como vacío u ocupado
            }
        }
    }
    public ItemSlots GetSlot(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return slots[x, y];
        }
        else
        {
            return null; // Retorna null si las coordenadas están fuera del rango
        }
    }
    public bool CanAddItem(Item item, int startX, int startY)
    {
        if (startX + item.width > width || startY + item.height > height)
            return false;

        for (int x = startX; x < startX + item.width; x++)
        {
            for (int y = startY; y < startY + item.height; y++)
            {
                if (slots[x, y].IsOccupied)
                    return false;
            }
        }
        return true;
    }

    public bool AddItem(Item item)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CanAddItem(item, x, y))
                {
                    Debug.Log($"Adding item at {x}, {y}");
                    PlaceItem(item, x, y);
                    return true;
                }
            }
        }
        Debug.Log("Failed to add item, no space available");
        return false;
    }
    private void PlaceItem(Item item, int startX, int startY)
    {
        // Asumimos que CanAddItem ya verificó que el espacio es suficiente
        for (int x = startX; x < startX + item.width; x++)
        {
            for (int y = startY; y < startY + item.height; y++)
            {
                slots[x, y].Occupy(item);  // Ocupa cada slot necesario con el item
            }
        }
        UpdateInventoryUI();
    }

    
    private void UpdateInventoryUI()
    {
        // Actualiza la UI si es necesario
        // Este método debería actualizar visualmente los slots del inventario

    }
}
