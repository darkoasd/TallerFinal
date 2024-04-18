using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // Singleton instance
    public GameObject inventoryUI;       // GameObject de tu inventario
   

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Más de una instancia de GameManager encontrada!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        bool isCurrentlyActive = inventoryUI.activeSelf;
        inventoryUI.SetActive(!isCurrentlyActive);

       
    }

}