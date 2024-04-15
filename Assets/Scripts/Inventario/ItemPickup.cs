using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public Item item; // Asigna esto en el inspector de Unity, asegurándote de que se trata de un objeto de tipo Item.


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Solo marca el item como disponible para recoger
            other.GetComponent<PlayerController>().currentItemPickup = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Marca el item como no disponible para recoger si el jugador se aleja
            other.GetComponent<PlayerController>().currentItemPickup = null;
        }
    }

    public void Pickup()
    {
        if (item != null)
        {
            Inventory.instance.AddItem(item); // Usa el singleton instance para acceder a Inventory
            Destroy(gameObject); // Destruye el objeto después de recogerlo
        }
        else
        {
            Debug.LogWarning("Attempted to pick up a null item.");
        }
    }
}
