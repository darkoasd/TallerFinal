using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Arma : MonoBehaviour
{
    //Stats
    public float daño = 10f; // Daño causado por el arma
    public float rango = 100f; // Rango máximo del arma
    public float tiempoEntreDisparos = 0.5f; // Tiempo mínimo entre disparos
    public int municionMaxima = 30; // Máxima munición
    protected float tiempoUltimoDisparo; // Control del tiempo desde el último disparo
    public float tiempoApuntar = 0.2f; // Tiempo para apuntar con precisión
    public bool apuntando = false; // Estado de apuntado

    //Aim
    public float precisionApuntado = 1f;  // Precisión perfecta cuando se apunta
    public float precisionDesdeCadera = 5f;  // Menor precisión al disparar desde la cadera
    public float precisionActual;
    public float recoil = 0.1f;  // El recoil incrementa la dispersión tras cada disparo
    public float recoilRecoveryRate = 0.1f;  // Tasa de recuperación del recoil



    public float umbralDeMiedo = 0.5f; // Umbral de miedo configurable desde el Editor

    public bool disparando = false; // Estado de disparo
    [Range(0, 1)] public float factorReduccionMiedo = 0.2f; //
    // Municion
    public int municionEnCargador;
    public int municionDeReserva;
    public int capacidadCargador = 30;
    //camera
    public Transform cameraTransform; // Referencia a la transformada de la cámara
    public float maxVerticalAngle = 10f; // Ángulo máximo hacia arriba
    public float minVerticalAngle = -10f; // Ángulo máximo hacia abajo
    //UI
    public GameObject crosshair;

    public Animator animator;
    protected virtual void Start()
    {
        // Inicialización existente...
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // Asegura que hay una referencia a la cámara
        }
        tiempoUltimoDisparo = -tiempoEntreDisparos;
        precisionActual = precisionDesdeCadera;
        municionEnCargador = capacidadCargador;
       
    }
   
    protected void AplicarRecoil()
    {
        precisionActual += recoil;
    }
    protected Vector3 CalcularDireccionDisparo()
    {
        Vector3 disparoDir = cameraTransform.forward;
        if (!apuntando)
        {
            float dispersionHorizontal = Random.Range(-precisionActual, precisionActual);
            float dispersionVertical = Random.Range(-precisionActual, precisionActual);
            disparoDir = Quaternion.Euler(dispersionVertical, dispersionHorizontal, 0) * disparoDir;
        }
        return disparoDir;
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
        if (crosshair != null)
            crosshair.SetActive(false);
        if (animator != null)
            animator.SetBool("Apuntando", true);
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
        if (crosshair != null)
            crosshair.SetActive(true);
        if (animator != null)
            animator.SetBool("Apuntando", false);
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
        if (Input.GetButtonDown("Fire2"))
        {
            ComenzarApuntar();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            DejarDeApuntar();
        }

        // Recuperación del recoil
        if (precisionActual > precisionDesdeCadera)
        {
            precisionActual -= recoilRecoveryRate * Time.deltaTime;
            precisionActual = Mathf.Max(precisionActual, apuntando ? precisionApuntado : precisionDesdeCadera);
        }
    }
}
