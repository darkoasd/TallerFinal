using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotsUI : MonoBehaviour
{
    public Image icon;  // Referencia al componente Image que muestra el �cono del �tem
    public int x, y;  // Coordenadas del slot en la cuadr�cula

    public void SetItem(Item item)
    {
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
    }
}
