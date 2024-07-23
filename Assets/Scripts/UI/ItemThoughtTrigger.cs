using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ItemThoughtTrigger : MonoBehaviour
{
    public string thoughtText; // Texto del pensamiento que aparecerá
    public TextMeshProUGUI thoughtDisplay; // Referencia al TextMeshPro en la UI
    public TextMeshProUGUI inspectPrompt; // Referencia al TextMeshPro para el mensaje de inspección
    public TextMeshProUGUI itemFoundMessage; // Referencia al TextMeshPro para el mensaje de ítem encontrado
    public Item itemToGive; // Item que se dará al inspeccionar
    public bool givesItem = false; // Controla si este objeto da un ítem

    private Inventario inventory; // Referencia al inventario
    private bool playerInRange = false;
    private bool thoughtShown = false;
    private bool itemCollected = false; // Para controlar si el item ya fue recogido

    private void Start()
    {
        // Encuentra el inventario al inicio para asegurarse de que la referencia se mantiene
        inventory = FindObjectOfType<Inventario>();
        if (inventory == null)
        {
            Debug.LogError("Inventory not found in the scene!");
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !thoughtShown)
        {
            ShowThought();
            if (givesItem && !itemCollected && itemToGive != null)
            {
                inventory.AddItem(itemToGive);
                itemCollected = true; // Marca el item como recogido
                ShowItemFoundMessage(); // Muestra el mensaje de ítem encontrado
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowInspectPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideInspectPrompt();
            HideThought();
        }
    }

    public void ShowThought()
    {
        if (thoughtDisplay != null)
        {
            thoughtDisplay.text = thoughtText;
            thoughtDisplay.gameObject.SetActive(true);
            thoughtShown = true;
            HideInspectPrompt(); // Oculta el mensaje de inspección
        }
    }

    public void HideThought()
    {
        if (thoughtDisplay != null)
        {
            thoughtDisplay.gameObject.SetActive(false);
            thoughtShown = false;
        }
    }

    public void ShowInspectPrompt()
    {
        if (inspectPrompt != null)
        {
            inspectPrompt.text = "Press E to inspect";
            inspectPrompt.gameObject.SetActive(true);
        }
    }

    public void HideInspectPrompt()
    {
        if (inspectPrompt != null)
        {
            inspectPrompt.gameObject.SetActive(false);
        }
    }

    public void ShowItemFoundMessage()
    {
        if (itemFoundMessage != null)
        {
            itemFoundMessage.text = "Encontraste una " + itemToGive.itemName + "escondida";
            itemFoundMessage.gameObject.SetActive(true);
            Invoke("HideItemFoundMessage", 3.0f); // Oculta el mensaje después de 3 segundos
        }
    }

    public void HideItemFoundMessage()
    {
        if (itemFoundMessage != null)
        {
            itemFoundMessage.gameObject.SetActive(false);
        }
    }

}