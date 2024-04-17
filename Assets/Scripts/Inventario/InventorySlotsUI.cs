using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotsUI : MonoBehaviour
{
    public Image icon;  // Referencia al componente Image que muestra el ícono del ítem
    public int x, y;  // Coordenadas del slot en la cuadrícula

    public void SetItem(Item item)
    {
        icon.sprite = item.icon;
        icon.enabled = true;

        // Ajustar el tamaño y posición del ícono
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(item.width * InventoryUI.instance.slotWidth, item.height * InventoryUI.instance.slotHeight);
        rt.anchoredPosition = new Vector2((item.width * InventoryUI.instance.slotWidth) / 2, -(item.height * InventoryUI.instance.slotHeight) / 2);
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
    }
}
