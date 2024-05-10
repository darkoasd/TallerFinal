using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryEnemy : Enemy
{
    public Transform hidePosition; // Punto espec�fico al que debe moverse antes de desaparecer

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
        // Espera hasta que el enemigo est� cerca del punto de ocultamiento
        while (!agente.pathPending && agente.remainingDistance > agente.stoppingDistance)
        {
            yield return null;
            yield return new WaitForSeconds(1f);  // Ajusta este tiempo seg�n sea necesario

            Disappear();  // Llama a la funci�n de desaparici�n
        }

        // Espera un peque�o retardo adicional si es necesario (por ejemplo, para una animaci�n)
   
    }

    void Disappear()
    {
        Debug.Log("El enemigo se asusta y desaparece");
        agente.isStopped = true;
        gameObject.SetActive(false);  // Opcionalmente, podr�as usar Destroy(gameObject); si no planeas reutilizar este enemigo
    }

    protected override void Patrullar()
    {
        // Implementar la l�gica de patrulla si es necesario
    }
}