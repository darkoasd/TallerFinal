using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    private bool isPlayerNear = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("Player is near the item.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerNear = false;
            Debug.Log("Player is no longer near the item.");
        }
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Trying to pick up the item.");
            PickupItem();
        }
    }

    private void PickupItem()
    {
        Inventario inventory = FindObjectOfType<Inventario>();
        if (inventory != null)
        {
            inventory.AddItem(item);
            Debug.Log("Item added to inventory.");
        }
        else
        {
            Debug.Log("Inventory not found.");
        }
        Destroy(gameObject);
    }
}