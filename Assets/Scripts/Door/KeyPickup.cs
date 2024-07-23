using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public KeyItem key; // Usa el objeto KeyItem en lugar de solo el keyId

    private bool isPlayerInTrigger = false;
    private KeyInventory playerInventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            playerInventory = other.GetComponent<KeyInventory>();
            if (playerInventory == null)
            {
                Debug.LogWarning("Player does not have a KeyInventory component.");
            }
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && playerInventory != null)
        {
            playerInventory.AddKey(key);
            Debug.Log("Llave recogida: " + key.itemName + " con ID: " + key.keyId);
            Destroy(gameObject); // Eliminar la llave del juego.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            playerInventory = null;
        }
    }
}