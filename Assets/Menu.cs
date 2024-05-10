using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
   public void CargarNivel1()
    {
        SceneManager.LoadScene("Nivel1");
        UpdateCursor(false);
    }
    public void SalirJuego()
    {
        Application.Quit();
    }
    void UpdateCursor(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
