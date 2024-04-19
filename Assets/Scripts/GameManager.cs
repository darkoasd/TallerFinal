using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance
    public GameObject inventoryUI;      // GameObject de tu inventario
    public GameObject gameOverScreen;
    public GameObject pauseMenuUI;      // GameObject para el men� de pausa

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("M�s de una instancia de GameManager encontrada!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        // Opcionalmente ocultar el cursor al inicio si el inventario inicia desactivado
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false); // Aseg�rate de que el men� de pausa est� desactivado al iniciar
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.P)) // Tecla t�pica para pausar el juego
        {
            TogglePauseMenu();
        }

        if (gameOverScreen.activeSelf) // Asegura que el cursor permanezca visible si Game Over est� activo
        {
            ShowCursor();
        }
    }
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);  // Oculta el men� de pausa
        Time.timeScale = 1;            // Reanuda el tiempo del juego
        Cursor.visible = false;        // Oculta el cursor
        Cursor.lockState = CursorLockMode.Locked;  // Bloquea el cursor al centro de la pantalla
    }
    void TogglePauseMenu()
    {
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf); // Toggle la visibilidad del men� de pausa

        if (pauseMenuUI.activeSelf)
        {
            Time.timeScale = 1 - Time.timeScale;
            if(Time.timeScale == 0)
            {
                ShowCursor();
            }
            
           
        }
        else
        {
            Time.timeScale = 1; // Reanuda el juego
            HideCursor();
        }
    }


    public void ToggleInventory()
    {
        bool isCurrentlyActive = inventoryUI.activeSelf;
        inventoryUI.SetActive(!isCurrentlyActive);

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.isInventoryOpen = !isCurrentlyActive;  // Actualizar la variable en PlayerController
        }

        // Configurar el cursor basado en si el inventario est� abierto o no
        if (isCurrentlyActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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

    public void CargarMundo()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
    public void GameOver()
    {
        gameOverScreen.SetActive(true); // Muestra la pantalla de Game Over
    }
    public void CargarMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

