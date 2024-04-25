using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EstadoEnemigo { Patrullando, Persiguiendo, Atacando, Investigando }
public abstract class Enemy : MonoBehaviour
{
    public float saludMaxima = 100f;
    public float da�oDeAtaque = 10f;
    public float velocidad = 3.5f;
    public float rangoDeVisi�n = 10f;
    public LayerMask capaDelJugador; // Define qu� capa(s) pertenece(n) al jugador para la detecci�n
    public LayerMask capaDeObst�culos; // Capa(s) que contienen obst�culos que bloquean la visi�n

    public float campoDeVision = 120f;

    public float saludActual;
    protected NavMeshAgent agente;
    protected Transform objetivo;
    public float distanciaDeParadaAtaque = 2f;

    //Attack
    public float tiempoEntreAtaques = 2f;
    private float temporizadorAtaque;
    public float rangoDeAtaque = 3f;
    public bool estaAtacando = false;

    public float tiempoDeRecuperacionDespuesDeAtaque = 2f;
    private float temporizadorRecuperacion;
    protected virtual void Start()
    {
        saludActual = saludMaxima;
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidad;
        objetivo = GameObject.FindGameObjectWithTag("Player").transform; // Asume que el jugador tiene el tag "Player"
    }

    protected virtual void Update()
    {

        if (temporizadorRecuperacion > 0)
        {
            temporizadorRecuperacion -= Time.deltaTime;
            if (temporizadorRecuperacion <= 0)
            {
                agente.isStopped = false; // Permite que el agente comience a moverse nuevamente
            }
            return; // Evita que el resto del c�digo se ejecute mientras el enemigo se est� recuperando
        }
        temporizadorAtaque += Time.deltaTime;
        if (objetivo == null) return;
        if (estaAtacando)
        {
            // Considera no hacer nada m�s si est� atacando, o manejar l�gicas espec�ficas de ataque aqu�
            return;
        }

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);

        Vector3 dirHaciaJugador = (objetivo.position - transform.position).normalized;
        float anguloHaciaJugador = Vector3.Angle(transform.forward, dirHaciaJugador);

        // Actualizar la distancia de parada seg�n si el enemigo est� listo para atacar o no
        agente.stoppingDistance = (distanciaAlJugador <= rangoDeAtaque && temporizadorAtaque >= tiempoEntreAtaques) ? distanciaDeParadaAtaque : 0f;

        if (distanciaAlJugador <= rangoDeVisi�n && anguloHaciaJugador <= campoDeVision / 2f && !estaAtacando)
        {
            if (!Physics.Linecast(transform.position, objetivo.position, capaDeObst�culos))
            {
                agente.SetDestination(objetivo.position);
            }
        }
        else
        {
            Patrullar();
        }

        if (distanciaAlJugador <= rangoDeAtaque && temporizadorAtaque >= tiempoEntreAtaques && !Physics.Linecast(transform.position, objetivo.position, capaDeObst�culos))
        {
            Atacar();
            temporizadorAtaque = 0f;
        }
    }
    protected void Atacar()
    {
        if (temporizadorAtaque < tiempoEntreAtaques) return; // Asegura que solo ataque cada cierto tiempo

        estaAtacando = true;
        agente.isStopped = true; // Detiene el movimiento del enemigo durante el ataque
        Debug.Log("Enemigo ataca");
        objetivo.GetComponent<PlayerController>().RecibirDa�o(da�oDeAtaque);

        temporizadorAtaque = 0; // Reinicia el temporizador de ataque

        StartCoroutine(EsperarDespuesDeAtacar());
    }
    IEnumerator EsperarDespuesDeAtacar()
    {
        yield return new WaitForSeconds(tiempoDeRecuperacionDespuesDeAtaque); // Tiempo de espera despu�s de atacar

        // Aqu� puedes opcionalmente mover el enemigo ligeramente para simular un "retroceso"
        MoverseAPosicionAleatoriaDespuesDeAtacar();

        yield return new WaitForSeconds(1f); // Espera adicional para permitir el reposicionamiento

        estaAtacando = false;
        agente.isStopped = false; // Permite que el enemigo vuelva a moverse
    }
    void MoverseAPosicionAleatoriaDespuesDeAtacar()
    {
        // Genera una direcci�n aleatoria para moverse
        Vector3 direccionAleatoria = Random.insideUnitSphere * 3f; // 3 metros de radio para el movimiento
        direccionAleatoria += transform.position;
        direccionAleatoria.y = transform.position.y; // Mant�n la misma altura

        NavMeshHit hit;
        if (NavMesh.SamplePosition(direccionAleatoria, out hit, 3f, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }

    protected void DetectarJugadorYPerseguir()
    {
        if (objetivo == null || estaAtacando) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);
        Vector3 dirHaciaJugador = (objetivo.position - transform.position).normalized;
        float anguloHaciaJugador = Vector3.Angle(transform.forward, dirHaciaJugador);

        // Suponiendo un campo de visi�n de 120 grados (60 grados a cada lado de la direcci�n en la que mira el enemigo)
        if (distanciaAlJugador <= rangoDeVisi�n && anguloHaciaJugador <= campoDeVision / 2f)
        {
            // Verifica si hay l�nea de visi�n directa con el jugador
            if (!Physics.Linecast(transform.position, objetivo.position, capaDeObst�culos))
            {
                // No hay obst�culos, sigue al jugador
                agente.SetDestination(objetivo.position);
            }
            else
            {
                // Hay un obst�culo entre el enemigo y el jugador, implementar comportamiento de patrulla o espera
                Patrullar();
            }
        }
        else
        {
            // Jugador fuera del campo de visi�n, implementar comportamiento de patrulla o espera
            Patrullar();
        }
    }
    void OnDrawGizmosSelected()
    {
        // Dibuja una l�nea hacia adelante desde el enemigo
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * rangoDeVisi�n);

        // Dibuja las l�neas del campo de visi�n
        Vector3 rightLimit = Quaternion.Euler(0, campoDeVision / 2, 0) * transform.forward * rangoDeVisi�n;
        Vector3 leftLimit = Quaternion.Euler(0, -campoDeVision / 2, 0) * transform.forward * rangoDeVisi�n;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, rightLimit);
        Gizmos.DrawRay(transform.position, leftLimit);

        // Dibuja un arco para el campo de visi�n
        Gizmos.color = Color.yellow;
        Vector3 startAngle = Quaternion.Euler(0, -campoDeVision / 2, 0) * transform.forward;
        Vector3 endAngle = Quaternion.Euler(0, campoDeVision / 2, 0) * transform.forward;
        DrawArc(transform.position, startAngle, endAngle, rangoDeVisi�n, 30);
    }

    // M�todo auxiliar para dibujar el arco del campo de visi�n
    void DrawArc(Vector3 center, Vector3 startDirection, Vector3 endDirection, float radius, int segments)
    {
        float angle = Vector3.Angle(startDirection, endDirection) / segments;
        Vector3 previousPoint = center + startDirection.normalized * radius;
        Vector3 nextPoint = Vector3.zero;

        for (int i = 1; i <= segments; i++)
        {
            nextPoint = center + (Quaternion.Euler(0, angle * i, 0) * startDirection.normalized) * radius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }

    protected abstract void Patrullar(); // M�todo abstracto para ser implementado por clases derivadas

    public void RecibirDa�o(float cantidad, Vector3 posicionDisparo)
    {
        saludActual -= cantidad;
        if (saludActual <= 0f)
        {
            Morir();
        }
        else
        {
            Investigar(posicionDisparo);
        }
    }
    protected void Investigar(Vector3 origenDisparo)
    {
        if (Vector3.Distance(transform.position, origenDisparo) > 5f)  // Solo reacciona si el disparo vino de lejos
        {
            agente.SetDestination(origenDisparo);
        }
    }

    protected virtual void Morir()
    {
        // Implementar l�gica de muerte, como reproducir animaci�n y destruir el objeto
        Destroy(gameObject);
    }
}