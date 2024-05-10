using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotePickup : MonoBehaviour
{
    public NoteItem note;
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
            playerInventory.AddNote(note);
            UIManager.Instance.ShowNote(note.noteText);
            Destroy(gameObject); // Eliminar el objeto de la nota
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

