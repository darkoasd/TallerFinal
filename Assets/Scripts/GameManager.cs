using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject inventoryUI;
    public GameObject inventoryArtifacts;
    public GameObject gameOverScreen;
    public GameObject pauseMenuUI;
    public GameObject inventoryButtons;

    public GameObject craftScreen;
    public GameObject craftScreenInfo;
    private bool isCraftActive = false;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ActivarCraft()
    {
        isCraftActive = !isCraftActive; 

        craftScreen.SetActive(isCraftActive);
        craftScreenInfo.SetActive(isCraftActive);
    }
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        HideInventory(inventoryArtifacts);
        inventoryButtons.SetActive(false);
    }

    public void OpenMainInventory()
    {
        Debug.Log("Opening Inventory");
        ShowInventory(inventoryUI);
        HideInventory(inventoryArtifacts);
        UpdateCursor(true);
        UpdatePlayerControllerInventoryStatus(true);
    }

    public bool IsInventoryOpen()
    {
        return inventoryUI.activeSelf || inventoryArtifacts.activeSelf;
    }

    public void OpenArtifactInventory()
    {
        HideInventory(inventoryUI);
        ShowInventory(inventoryArtifacts);
        UpdateCursor(true);
        UpdatePlayerControllerInventoryStatus(true);
    }

    public void OpenCraftingInventory()
    {
        HideInventory(inventoryUI);
        HideInventory(inventoryArtifacts);
        UpdateCursor(true);
        UpdatePlayerControllerInventoryStatus(true);
    }

    public void CloseInventories()
    {
        HideInventory(inventoryUI);
        HideInventory(inventoryArtifacts);
        UpdateCursor(false);
        UpdatePlayerControllerInventoryStatus(false);
    }

    void ShowInventory(GameObject inventory)
    {
        inventory.SetActive(true); // Asegúrate de que el inventario está activo
        foreach (Transform child in inventory.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    void HideInventory(GameObject inventory)
    {
        foreach (Transform child in inventory.transform)
        {
            child.gameObject.SetActive(false);
        }
        inventory.SetActive(false); // Asegúrate de que el inventario está inactivo
    }

    bool IsInventoryVisible(GameObject inventory)
    {
        return inventory.activeSelf;
    }

    void UpdateCursor(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void UpdatePlayerControllerInventoryStatus(bool isOpen)
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.isInventoryOpen = isOpen;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void ToggleInventory()
    {
        if (IsInventoryOpen())
        {
            CloseInventories();
        }
        else
        {
            OpenMainInventory();
        }

        inventoryButtons.SetActive(IsInventoryOpen());
    }

    void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        UpdateCursor(false);
    }

    void TogglePauseMenu()
    {
        bool isActive = pauseMenuUI.activeSelf;
        pauseMenuUI.SetActive(!isActive);
        Time.timeScale = isActive ? 1 : 0;

        UpdateCursor(!isActive);
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        UpdateCursor(true);
    }

    public void CargarMundo()
    {
        SceneManager.LoadScene("Nivel1");
        StartCoroutine(ResetPlayerStateAfterLoad());
    }

    IEnumerator ResetPlayerStateAfterLoad()
    {
        yield return null;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            gameOverScreen.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController.Reiniciar();
        }
    }

    public void SalirJuego()
    {
        Application.Quit();
    }

    public void CargarMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        gameOverScreen.SetActive(false);
        pauseMenuUI.SetActive(false);
        HideInventory(inventoryUI);
        HideInventory(inventoryArtifacts);
    }
}