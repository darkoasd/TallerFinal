using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject inventoryUI;
    public GameObject inventoryArtifacts; // GameObject para el inventario de artefactos
    public GameObject gameOverScreen;
    public GameObject pauseMenuUI;
    public GameObject inventoryButtons;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // Destruye el nuevo objeto si ya existe una instancia
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
       
        inventoryArtifacts.SetActive(false);
    }
    public void OpenMainInventory()
    {
        Debug.Log("Opening Inventory");
        inventoryUI.SetActive(true);
        inventoryArtifacts.SetActive(false);
        UpdateCursor(true);
        UpdatePlayerControllerInventoryStatus(true);
    }
    public bool IsInventoryOpen()
    {
        return inventoryUI.activeSelf || inventoryArtifacts.activeSelf;
    }

    public void OpenArtifactInventory()
    {
        inventoryUI.SetActive(false);
        inventoryArtifacts.SetActive(true);
        UpdateCursor(true);
        UpdatePlayerControllerInventoryStatus(true);
    }

    public void CloseInventories()
    {
        inventoryUI.SetActive(false);
        inventoryArtifacts.SetActive(false);
        UpdateCursor(false);
        UpdatePlayerControllerInventoryStatus(false);
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
        bool anyInventoryActive = inventoryUI.activeSelf || inventoryArtifacts.activeSelf;

        if (anyInventoryActive)
        {
            inventoryUI.SetActive(false);
            inventoryArtifacts.SetActive(false);
            inventoryButtons.SetActive(false);
            UpdateCursor(false);
            UpdatePlayerControllerInventoryStatus(false);
        }
        else
        {
            inventoryUI.SetActive(true);
            inventoryArtifacts.SetActive(false);
            inventoryButtons.SetActive(true);
            UpdateCursor(true);
            UpdatePlayerControllerInventoryStatus(true);
        }
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
        // Espera un frame después de cargar la escena para que todos los scripts se hayan inicializado
        yield return null;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            gameOverScreen.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController.Reiniciar(); // Reinicia el estado del jugador
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
        inventoryUI.SetActive(false);
        inventoryArtifacts.SetActive(false);
    }
}