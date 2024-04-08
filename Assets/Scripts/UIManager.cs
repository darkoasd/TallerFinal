using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void AbrirJuego()
    {
        SceneManager.LoadScene("Nivel1");
     
    }
    public void SalirJuego()
    {
        Application.Quit();
    }
}
