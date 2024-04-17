using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Arma : MonoBehaviour
{
    public float da�o = 10f; // Da�o causado por el arma
    public float rango = 100f; // Rango m�ximo del arma
    public float tiempoEntreDisparos = 0.5f; // Tiempo m�nimo entre disparos
    public int municionMaxima = 30; // M�xima munici�n
    protected float tiempoUltimoDisparo; // Control del tiempo desde el �ltimo disparo
    public float tiempoApuntar = 0.2f; // Tiempo para apuntar con precisi�n
    public bool apuntando = false; // Estado de apuntado
    public float precisionApuntado; // Mejora la precisi�n mientras se apunta
    public float precisionDesdeCadera; // Precisi�n base sin apuntar
    public float precisionActual;

    public float umbralDeMiedo = 0.5f; // Umbral de miedo configurable desde el Editor
    [Range(0, 1)] public float factorReduccionMiedo = 0.2f; //
    // Municion
    public int municionEnCargador;
    public int municionDeReserva;
    public int capacidadCargador = 30;
    //camera
    public Transform cameraTransform; // Referencia a la transformada de la c�mara
    public float maxVerticalAngle = 10f; // �ngulo m�ximo hacia arriba
    public float minVerticalAngle = -10f; // �ngulo m�ximo hacia abajo
    //UI
    public GameObject crosshair;

    public Animator animator;
    protected virtual void Start()
    {
        // Inicializaci�n existente...
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // Asegura que hay una referencia a la c�mara
        }
        tiempoUltimoDisparo = -tiempoEntreDisparos;
        precisionActual = precisionDesdeCadera;
        municionEnCargador = capacidadCargador;
        municionDeReserva = 90; // Asumiendo que empiezas con 90 balas de reserva.
    }
    public void AjustarPrecision(float nivelDeMiedo)
    {

        if (nivelDeMiedo > umbralDeMiedo)
        {
            // Usa el factorReduccionMiedo para determinar la nueva precisi�n
            precisionActual = precisionDesdeCadera * factorReduccionMiedo;
        }
        else
        {
            // Mantener la precisi�n base si el nivel de miedo est� por debajo del umbral
            precisionActual = precisionDesdeCadera;
        }

        // Ajustar el tama�o del crosshair basado en la precisi�n actual
        if (crosshair != null)
        {
            // Calcula el tama�o del crosshair basado en la precisi�n
           
            float factorDeCambio = 1 / factorReduccionMiedo; // Inversamente proporcional al factor de reducci�n
            float tama�o = Mathf.Clamp(factorDeCambio, 1, 3); // Limita el tama�o m�ximo
            crosshair.transform.localScale = new Vector3(tama�o, tama�o, 1);
        }
        Debug.Log($"Precisi�n Actual: {precisionActual}");
    }
    public void ToggleApuntar()
    {
        apuntando = !apuntando;
        precisionActual = apuntando ? precisionApuntado : precisionDesdeCadera;

        // Actualiza la visibilidad del crosshair y el estado de la animaci�n
        if (crosshair != null)
        {
            crosshair.SetActive(!apuntando);
        }

        if (animator != null)
        {
            animator.SetBool("Apuntando", apuntando);
        }
    }

    // M�todo para disparar (ser� sobreescrito por clases hijas)
    public abstract void Disparar();

    // M�todo para comenzar a apuntar
    public void ComenzarApuntar()
    {
        apuntando = true;
        precisionActual = precisionApuntado;

        // Oculta el crosshair cuando el jugador est� apuntando
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        // Cambia la animaci�n a apuntando
        if (animator != null)
        {
            animator.SetBool("Apuntando", true);
        }
    }
    public void ActualizarEstadoDeApuntado(bool estaApuntando, float nivelDeMiedo, float maxMiedo)
    {
        apuntando = estaApuntando;
        precisionActual = estaApuntando ? precisionApuntado : precisionDesdeCadera;
    }
    public void DejarDeApuntar()
    {
        apuntando = false;
        precisionActual = precisionDesdeCadera;

        // Muestra el crosshair cuando el jugador no est� apuntando
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
        // Cambia la animaci�n a idle
        if (animator != null) animator.SetBool("Apuntando", false);
    }

    // Corrutina para simular demora al apuntar
    IEnumerator ApuntarConDemora()
    {
        yield return new WaitForSeconds(tiempoApuntar);
        apuntando = true;
    }

    // M�todo para dejar de apuntar
   

    // Este m�todo actualizar� el estado del arma cada frame
    protected virtual void Update()
    {
      

        // Comprueba si el jugador est� presionando o soltando el bot�n derecho del rat�n
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
