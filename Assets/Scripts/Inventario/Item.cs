using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite icon;
    public int width;  // Ancho en casillas
    public int height; // Alto en casillas

    public virtual void Use()
    {
        Debug.Log("Using " + itemName);
    }
}
