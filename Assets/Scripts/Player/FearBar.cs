using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FearBar : MonoBehaviour
{
    public Slider fearSlider; // Referencia al Slider de la UI
    public PlayerController playerController; // Referencia al script PlayerController

    void Update()
    {
        if (playerController != null && fearSlider != null)
        {
            // Ajusta el valor del Slider para que coincida con el nivel de miedo del jugador
            fearSlider.value = playerController.nivelDeMiedo / playerController.maxMiedo;
        }
    }
}
