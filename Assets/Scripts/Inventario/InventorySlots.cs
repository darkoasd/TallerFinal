using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public int x, y;  // Posiciones del slot en la cuadrícula
    public Button slotButton;
    void Start()
    {
        slotButton.onClick.AddListener(() => SelectSlot());
    }

    void SelectSlot()
    {
        InventoryUI.instance.SelectSlot(x, y);
    }
}
