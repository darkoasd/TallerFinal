using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public Vector2Int size; // Tamaño para el inventario.

    // Método para rotar el tamaño del ítem en el inventario
    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x); // Intercambia las dimensiones x e y
    }
}