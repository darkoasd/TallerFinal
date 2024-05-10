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
        agente.isStopped = true; // Detiene el movimiento inicialmente
        ultimaPosicionConocida = Vector3.zero;
    }

    protected override void Update()
    {
        base.Update();

        if (!jugadorDetectado)
        {
            DetectarJugadorYPerseguir();
        }
        else
        {
            PerseguirUltimaPosicionConocida();
        }
    }

    protected override void DetectarJugadorYPerseguir()
    {
        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);
        Vector3 direccionHaciaJugador = (objetivo.position - transform.position).normalized;
        float anguloHaciaJugador = Vector3.Angle(transform.forward, direccionHaciaJugador);

        if (distanciaAlJugador <= rangoDeVisión && anguloHaciaJugador <= campoDeVision / 2f)
        {
            if (!Physics.Linecast(transform.position, objetivo.position, capaDeObstáculos))
            {
                jugadorDetectado = true;
                ultimaPosicionConocida = objetivo.position;
                agente.isStopped = false;
                agente.SetDestination(objetivo.position);
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
        if (jugadorDetectado || ultimaPosicionConocida == Vector3.zero) return;

        if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            agente.isStopped = true; // Detiene al enemigo si llega al último punto conocido y no ve al jugador
        }
        else
        {
            agente.SetDestination(ultimaPosicionConocida);
        }
    }

    protected override void Patrullar()
    {
        // Este enemigo no patrulla, por lo que este método permanece vacío.
    }
}