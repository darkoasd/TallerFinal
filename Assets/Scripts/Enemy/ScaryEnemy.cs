using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryEnemy : Enemy
{
    public Transform hidePosition; // Punto específico al que debe moverse antes de desaparecer

    public void TriggerHide()
    {
        if (hidePosition != null)
        {
            agente.SetDestination(hidePosition.position);
            agente.isStopped = false;
            StartCoroutine(CheckArrivalAndDisappear());
        }
        else
        {
            Debug.LogError("Hide position not set for ScaryEnemy.");
            Disappear();
        }
    }

    IEnumerator CheckArrivalAndDisappear()
    {
        // Espera hasta que el enemigo esté cerca del punto de ocultamiento
        while (!agente.pathPending && agente.remainingDistance > agente.stoppingDistance)
        {
            yield return null;
            yield return new WaitForSeconds(1f);  // Ajusta este tiempo según sea necesario

            Disappear();  // Llama a la función de desaparición
        }

        // Espera un pequeño retardo adicional si es necesario (por ejemplo, para una animación)
   
    }

    void Disappear()
    {
        Debug.Log("El enemigo se asusta y desaparece");
        agente.isStopped = true;
        gameObject.SetActive(false);  // Opcionalmente, podrías usar Destroy(gameObject); si no planeas reutilizar este enemigo
    }

    protected override void Patrullar()
    {
        // Implementar la lógica de patrulla si es necesario
    }
}