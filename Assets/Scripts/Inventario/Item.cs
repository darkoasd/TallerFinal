using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public Vector2Int size; // Tama�o para el inventario.

    // M�todo para rotar el tama�o del �tem en el inventario
    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x); // Intercambia las dimensiones x e y
    }
}