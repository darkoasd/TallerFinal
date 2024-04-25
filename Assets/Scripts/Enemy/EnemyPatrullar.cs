using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrullar : Enemy
{
    public float radioDePatrulla = 10f;
    private Vector3 puntoDePatrulla;
    public float tiempoEntrePuntos;
    private bool buscandoPunto = false;



    protected override void Start()
    {
        base.Start();
        ElegirPuntoDePatrulla();
    }

    protected override void Patrullar()
    {
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            ElegirPuntoDePatrulla();
        }
    }
    protected override void Update()
    {
        base.Update();

        if (agente.remainingDistance < agente.stoppingDistance)
        {
            Patrullar();  // Continúa patrullando si ha llegado al destino y no ve al jugador
        }
    }

    void ElegirPuntoDePatrulla()
    {
        StartCoroutine(EsperarYBuscarNuevoPunto());
    }
    IEnumerator EsperarYBuscarNuevoPunto()
    {
        if (buscandoPunto) yield break; // Sale si ya está buscando un punto
        buscandoPunto = true;
        // Espera un tiempo determinado
        yield return new WaitForSeconds(tiempoEntrePuntos); // Espera 2 segundos antes de buscar un nuevo punto

        // Calcula un punto aleatorio dentro del radio de patrulla
        Vector3 puntoAleatorio = Random.insideUnitSphere * radioDePatrulla + transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(puntoAleatorio, out hit, radioDePatrulla, -1))
        {
            puntoDePatrulla = hit.position;
            agente.SetDestination(puntoDePatrulla);
        }
        buscandoPunto = false; // Restablece el estado
    }
    /*void OnDrawGizmosSelected()
    {
        // Dibuja un círculo alrededor del enemigo para representar el radio de patrulla
        Gizmos.color = Color.yellow; // Establece el color del Gizmo a amarillo (o cualquier color que prefieras)
        Gizmos.DrawWireSphere(transform.position, radioDePatrulla);
    }*/
    void OnDrawGizmos()
    {
        // Asegúrate de que solo se dibuja si realmente hay un punto de patrulla establecido
        // Esto verifica que el punto de patrulla no sea el vector cero (o cualquier otro valor que uses para indicar que no hay punto establecido)
        if (puntoDePatrulla != Vector3.zero)
        {
            Gizmos.color = Color.green; // Elige un color para el Gizmo
            Gizmos.DrawSphere(puntoDePatrulla, 1); // Dibuja una esfera en el punto de patrulla
        }
    }
}