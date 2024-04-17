using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public Item item; // Asigna esto en el inspector de Unity, asegurándote de que se trata de un objeto de tipo Item.

    private bool isPlayerNearby = false;
    private PlayerController playerController;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerController = other.GetComponent<PlayerController>();
            playerController.currentItemPickup = this;  // Opcional, depende de tu lógica
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerController.currentItemPickup = null;  // Opcional, depende de tu lógica
        }
    }

    public void Pickup()
    {
        if (item != null && Inventory.instance.AddItem(item))  // Asumiendo que AddItem ahora devuelve true si fue exitoso
        {
            Destroy(gameObject);  // Destruye el objeto después de recogerlo
        }
        else
        {
            Debug.LogWarning("No space to pick up the item or item is null.");
        }
    }

}
