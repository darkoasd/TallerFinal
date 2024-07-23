using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatic : Enemy
{
    private bool jugadorDetectado = false;
    private Vector3 ultimaPosicionConocida;

    protected override void Start()
    {
        base.Start();
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true; // Detiene el movimiento inicialmente
        }
        ultimaPosicionConocida = Vector3.zero;
    }

    protected override void Update()
    {
        if (!isAlive) return; // Asegurarse de que no se ejecuta si el enemigo no está vivo

        if (jugadorDetectado)
        {
            PerseguirUltimaPosicionConocida();
        }
        else
        {
            base.Update();
        }
    }

    protected override void DetectarJugadorYPerseguir()
    {
        if (!isAlive) return; // Asegurarse de que no se ejecuta si el enemigo no está vivo

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);
        Vector3 direccionHaciaJugador = (objetivo.position - transform.position).normalized;
        float anguloHaciaJugador = Vector3.Angle(transform.forward, direccionHaciaJugador);

        Debug.Log("EnemyStatic detectando jugador: Distancia " + distanciaAlJugador + ", Angulo " + anguloHaciaJugador);

        if (distanciaAlJugador <= rangoDeVisión && anguloHaciaJugador <= campoDeVision / 2f)
        {
            if (!Physics.Linecast(transform.position, objetivo.position, capaDeObstáculos))
            {
                jugadorDetectado = true;
                ultimaPosicionConocida = objetivo.position;
                if (agente != null && agente.isOnNavMesh)
                {
                    agente.isStopped = false;
                    agente.SetDestination(ultimaPosicionConocida);
                    Debug.Log("EnemyStatic: Jugador detectado, persiguiendo");
                }
            }
            else
            {
                jugadorDetectado = false;
            }
        }
        else
        {
            jugadorDetectado = false;
        }
    }

    void PerseguirUltimaPosicionConocida()
    {
        if (!jugadorDetectado || ultimaPosicionConocida == Vector3.zero || !isAlive) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);
        Vector3 direccionHaciaJugador = (objetivo.position - transform.position).normalized;
        float anguloHaciaJugador = Vector3.Angle(transform.forward, direccionHaciaJugador);

        if (distanciaAlJugador <= rangoDeVisión && anguloHaciaJugador <= campoDeVision / 2f &&
            !Physics.Linecast(transform.position, objetivo.position, capaDeObstáculos))
        {
            ultimaPosicionConocida = objetivo.position;
        }

        if (!agente.pathPending && agente.isOnNavMesh && agente.remainingDistance <= agente.stoppingDistance)
        {
            agente.isStopped = true; // Detiene al enemigo si llega al último punto conocido y no ve al jugador
            jugadorDetectado = false; // Reset jugadorDetectado para reactivar la detección
        }
        else
        {
            if (agente != null && agente.isOnNavMesh)
            {
                agente.SetDestination(ultimaPosicionConocida);
            }
        }
    }

    protected override void Patrullar()
    {
        // Este enemigo no patrulla, por lo que este método permanece vacío.
    }

    public override void RecibirDaño(float cantidad, Vector3 posicionDisparo, string parteDelCuerpo)
    {
        if (!isAlive) return;

        base.RecibirDaño(cantidad, posicionDisparo, parteDelCuerpo);
        jugadorDetectado = true;
        ultimaPosicionConocida = posicionDisparo;
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = false;
            agente.SetDestination(ultimaPosicionConocida);
        }
    }
}