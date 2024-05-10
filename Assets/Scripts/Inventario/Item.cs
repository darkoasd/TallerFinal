using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Weapon,
    Consumable,
    General
}
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public Vector2Int size;
    public GameObject itemPrefab;  
    public ItemType itemType;
    public int healingAmount;


    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x); 
    }
}