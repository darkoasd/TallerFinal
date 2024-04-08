using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Arma : MonoBehaviour
{
    public float daño = 10f; // Daño causado por el arma
    public float rango = 100f; // Rango máximo del arma
    public float tiempoEntreDisparos = 0.5f; // Tiempo mínimo entre disparos
    public int municionMaxima = 30; // Máxima munición
    protected float tiempoUltimoDisparo; // Control del tiempo desde el último disparo
    public float tiempoApuntar = 0.2f; // Tiempo para apuntar con precisión
    public bool apuntando = false; // Estado de apuntado
    public float precisionApuntado; // Mejora la precisión mientras se apunta
    public float precisionDesdeCadera; // Precisión base sin apuntar
    public float precisionActual;

    public float umbralDeMiedo = 0.5f; // Umbral de miedo configurable desde el Editor
    [Range(0, 1)] public float factorReduccionMiedo = 0.2f; //
    // Municion
    public int municionEnCargador;
    public int municionDeReserva;
    public int capacidadCargador = 30;

    //UI
    public GameObject crosshair;
    
    public Animator animator;
    protected virtual void Start()
    {
        tiempoUltimoDisparo = -tiempoEntreDisparos;
        precisionActual = precisionDesdeCadera;
        municionEnCargador = capacidadCargador;
        municionDeReserva = 90; // Asumiendo que empiezas con 90 balas de reserva.
    }
    public void AjustarPrecision(float nivelDeMiedo)
    {

        if (nivelDeMiedo > umbralDeMiedo)
        {
            // Usa el factorReduccionMiedo para determinar la nueva precisión
            precisionActual = precisionDesdeCadera * factorReduccionMiedo;
        }
        else
        {
            // Mantener la precisión base si el nivel de miedo está por debajo del umbral
            precisionActual = precisionDesdeCadera;
        }

        // Ajustar el tamaño del crosshair basado en la precisión actual
        if (crosshair != null)
        {
            // Calcula el tamaño del crosshair basado en la precisión
           
            float factorDeCambio = 1 / factorReduccionMiedo; // Inversamente proporcional al factor de reducción
            float tamaño = Mathf.Clamp(factorDeCambio, 1, 3); // Limita el tamaño máximo
            crosshair.transform.localScale = new Vector3(tamaño, tamaño, 1);
        }
        Debug.Log($"Precisión Actual: {precisionActual}");
    }
    public void ToggleApuntar()
    {
        apuntando = !apuntando;
        precisionActual = apuntando ? precisionApuntado : precisionDesdeCadera;

        // Actualiza la visibilidad del crosshair y el estado de la animación
        if (crosshair != null)
        {
            crosshair.SetActive(!apuntando);
        }

        if (animator != null)
        {
            animator.SetBool("Apuntando", apuntando);
        }
    }

    // Método para disparar (será sobreescrito por clases hijas)
    public abstract void Disparar();

    // Método para comenzar a apuntar
    public void ComenzarApuntar()
    {
        apuntando = true;
        precisionActual = precisionApuntado;
        // Cambia la escala del crosshair
        if (crosshair != null)
        {
            crosshair.transform.localScale = new Vector3(1, 1, 1); // Cambia la escala del crosshair a 1 cuando el jugador está apuntando
        }
        // Cambia la animación a apuntando
        if (animator != null) animator.SetBool("Apuntando", true);
    }
    public void ActualizarEstadoDeApuntado(bool estaApuntando, float nivelDeMiedo, float maxMiedo)
    {
        apuntando = estaApuntando;

        // Si está apuntando y el nivel de miedo es bajo, establece precisión perfecta
        if (apuntando && nivelDeMiedo <= umbralDeMiedo)
        {
            precisionActual = 1.0f; // Asume que 1.0f representa la precisión perfecta
        }
        else if (apuntando) // Si está apuntando pero tiene algo de miedo
        {
            // Ajusta la precisión según el nivel de miedo, aún cuando se está apuntando
            precisionActual = Mathf.Lerp(precisionApuntado, precisionDesdeCadera, nivelDeMiedo / maxMiedo);
        }
        else if (nivelDeMiedo > umbralDeMiedo) // No está apuntando y tiene miedo
        {
            precisionActual = precisionDesdeCadera * factorReduccionMiedo;
        }
        else // Caso base, no está apuntando y no tiene miedo
        {
            precisionActual = precisionDesdeCadera;
        }
    }
    public void DejarDeApuntar()
    {
        apuntando = false;
        precisionActual = precisionDesdeCadera;
        // Cambia la escala del crosshair
        if (crosshair != null)
        {
            crosshair.transform.localScale = new Vector3(2, 2, 2); // Cambia la escala del crosshair a 2 cuando el jugador no está apuntando
        }
        // Cambia la animación a idle
        if (animator != null) animator.SetBool("Apuntando", false);
    }

    // Corrutina para simular demora al apuntar
    IEnumerator ApuntarConDemora()
    {
        yield return new WaitForSeconds(tiempoApuntar);
        apuntando = true;
    }

    // Método para dejar de apuntar
   

    // Este método actualizará el estado del arma cada frame
    protected virtual void Update()
    {
        // Comprueba si el jugador está presionando o soltando el botón derecho del ratón
        if (Input.GetButtonDown("Fire2"))
        {
            ComenzarApuntar();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            DejarDeApuntar();
        }
    }
}
