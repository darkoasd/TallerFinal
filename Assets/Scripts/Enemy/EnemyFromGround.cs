using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFromGround : Enemy
{
    public Transform puntoDeAparicion;
    public Collider triggerCollider; // Referencia al Collider externo como trigger

    private bool haAparecido = false;

    protected override void Start()
    {
        base.Start();
        agente.enabled = false; // Desactiva el NavMeshAgent inicialmente hasta que el enemigo aparezca
        transform.position = puntoDeAparicion.position; // Posición inicial, podría ser debajo del nivel del suelo para simular que "emerge"

        // Configura el collider externo como trigger y añade el script que maneja el evento
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
            TriggerEnemyFromGround handler = triggerCollider.GetComponent<TriggerEnemyFromGround>();
            if (handler == null)
            {
                handler = triggerCollider.gameObject.AddComponent<TriggerEnemyFromGround>();
            }
            handler.PlayerEnterTrigger += PlayerEnteredTrigger; // Suscripción al evento
        }
    }

    private void PlayerEnteredTrigger()
    {
        if (!haAparecido)
        {
            Aparecer();
        }
    }

    private void Aparecer()
    {
        haAparecido = true;
        agente.enabled = true; // Activa el NavMeshAgent para permitir el movimiento
        Debug.Log("Enemigo aparece desde el suelo!");
    }

    protected override void Patrullar()
    {
       
    }

    protected override void Morir()
    {
        base.Morir();
        // Implementar cualquier lógica adicional al morir, como efectos especiales
    }

    void OnDestroy()
    {
        if (triggerCollider != null)
        {
            TriggerEnemyFromGround handler = triggerCollider.GetComponent<TriggerEnemyFromGround>();
            if (handler != null)
            {
                handler.PlayerEnterTrigger -= PlayerEnteredTrigger; // Desuscripción del evento
            }
        }
    }
}


