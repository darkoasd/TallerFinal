using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject inventoryUI; // Arrastra aquí el GameObject de tu inventario desde el Editor

    // Asegúrate de que el inventario esté cerrado al inicio
    void Start()
    {
        inventoryUI.SetActive(false);
    }

    // Actualiza se llama una vez por frame
    void Update()
    {
        // Escucha la tecla TAB para abrir/cerrar el inventario
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    // Función para abrir/cerrar el inventario
    public void ToggleInventory()
    {
        bool isCurrentlyActive = inventoryUI.activeSelf;
        inventoryUI.SetActive(!isCurrentlyActive);
    }
}
