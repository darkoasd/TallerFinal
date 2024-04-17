using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlots : MonoBehaviour
{
    
    public bool IsOccupied { get; private set; }
    public Item Item { get; private set; }

    public void Occupy(Item item)
    {
        Item = item;
        IsOccupied = true;
    }

    public void Clear()
    {
        Item = null;
        IsOccupied = false;
    }
}
