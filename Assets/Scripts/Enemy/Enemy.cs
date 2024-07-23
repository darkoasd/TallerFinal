using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EstadoEnemigo
{
    Patrullando,
    Persiguiendo,
    Atacando,
    Investigando
}

public abstract class Enemy : MonoBehaviour
{
    public float saludMaxima = 100f;
    public float da�oDeAtaque = 10f;
    public float velocidad = 3.5f;
    public float rangoDeVisi�n = 10f;
    public LayerMask capaDelJugador;
    public LayerMask capaDeObst�culos;

    public float campoDeVision = 120f;
    public float saludActual;
    protected NavMeshAgent agente;
    protected Transform objetivo;
    public float distanciaDeParadaAtaque = 2f;

    public float tiempoEntreAtaques = 2f;
    private float temporizadorAtaque;
    public float rangoDeAtaque = 3f;
    public bool estaAtacando = false;

    public float tiempoDeRecuperacionDespuesDeAtaque = 2f;
    private float temporizadorRecuperacion;

    public EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;
    protected bool isAlive = true;

    // Nueva parte para los sonidos
    public AudioClip[] sonidos;
    public float intervaloSonidos = 5f;
    private float temporizadorSonidos;
    private AudioSource audioSource;

    public float audioVolume = 1f;
    public float audioMinDistance = 1f;
    public float audioMaxDistance = 10f;

    protected virtual void Start()
    {
        saludActual = saludMaxima;
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidad;
        objetivo = GameObject.FindGameObjectWithTag("Player").transform;

        // Inicializaci�n de AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = audioVolume;
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.minDistance = audioMinDistance;
        audioSource.maxDistance = audioMaxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear; // O usar AudioRolloffMode.Logarithmic
        audioSource.dopplerLevel = 0;

        temporizadorSonidos = intervaloSonidos;
    }

    protected virtual void Update()
    {
        if (!isAlive) return;

        temporizadorAtaque += Time.deltaTime;
        temporizadorRecuperacion -= Time.deltaTime;
        temporizadorSonidos -= Time.deltaTime;

        if (temporizadorRecuperacion <= 0 && agente.isOnNavMesh)
        {
            agente.isStopped = false;
        }

        if (temporizadorSonidos <= 0)
        {
            ReproducirSonidoAleatorio();
            temporizadorSonidos = intervaloSonidos;
        }

        switch (estadoActual)
        {
            case EstadoEnemigo.Patrullando:
                Patrullar();
                DetectarJugadorYPerseguir();
                break;
            case EstadoEnemigo.Persiguiendo:
                PerseguirJugador();
                break;
            case EstadoEnemigo.Atacando:
                break;
            case EstadoEnemigo.Investigando:
                break;
        }
    }

    protected void ReproducirSonidoAleatorio()
    {
        if (sonidos.Length > 0)
        {
            int index = Random.Range(0, sonidos.Length);
            audioSource.clip = sonidos[index];
            audioSource.Play();
        }
    }

    protected void PerseguirJugador()
    {
        if (estaAtacando || !isAlive) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);
        if (distanciaAlJugador <= rangoDeAtaque && temporizadorAtaque >= tiempoEntreAtaques)
        {
            Atacar();
            temporizadorAtaque = 0f;
        }
        else
        {
            if (agente.isOnNavMesh)
            {
                agente.SetDestination(objetivo.position);
            }
        }

        if (distanciaAlJugador > rangoDeVisi�n)
        {
            estadoActual = EstadoEnemigo.Patrullando;
        }
    }

    protected void Atacar()
    {
        if (temporizadorAtaque < tiempoEntreAtaques || !isAlive) return;

        estaAtacando = true;
        agente.isStopped = true;
        objetivo.GetComponent<PlayerController>().RecibirDa�o(da�oDeAtaque);
        temporizadorRecuperacion = tiempoDeRecuperacionDespuesDeAtaque;
        StartCoroutine(EsperarDespuesDeAtacar());
    }

    IEnumerator EsperarDespuesDeAtacar()
    {
        yield return new WaitForSeconds(tiempoDeRecuperacionDespuesDeAtaque);
        MoverseAPosicionAleatoriaDespuesDeAtacar();
        yield return new WaitForSeconds(1f);
        estaAtacando = false;
    }

    void MoverseAPosicionAleatoriaDespuesDeAtacar()
    {
        if (!isAlive) return;

        Vector3 direccionAleatoria = Random.insideUnitSphere * 3f;
        direccionAleatoria += transform.position;
        direccionAleatoria.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(direccionAleatoria, out hit, 3f, NavMesh.AllAreas))
        {
            if (agente.isOnNavMesh)
            {
                agente.SetDestination(hit.position);
            }
        }
    }

    protected virtual void DetectarJugadorYPerseguir()
    {
        if (objetivo == null || estaAtacando || !isAlive) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);
        Vector3 dirHaciaJugador = (objetivo.position - transform.position).normalized;
        float anguloHaciaJugador = Vector3.Angle(transform.forward, dirHaciaJugador);

        if (distanciaAlJugador <= rangoDeVisi�n && anguloHaciaJugador <= campoDeVision / 2f)
        {
            if (!Physics.Linecast(transform.position, objetivo.position, capaDeObst�culos))
            {
                estadoActual = EstadoEnemigo.Persiguiendo;
            }
        }
    }

    protected abstract void Patrullar();

    public virtual void RecibirDa�o(float cantidad, Vector3 posicionDisparo, string parteDelCuerpo)
    {
        if (!isAlive) return;

        float da�oFinal = cantidad;
        if (parteDelCuerpo == "Cabeza")
        {
            da�oFinal *= 2;
        }

        saludActual -= da�oFinal;
        if (saludActual <= 0f)
        {
            Morir();
        }
        else
        {
            estadoActual = EstadoEnemigo.Persiguiendo;
            if (agente.isOnNavMesh && isAlive)
            {
                agente.SetDestination(posicionDisparo);
            }
        }
    }

    protected virtual void Morir()
    {
        isAlive = false;
        if (agente.isOnNavMesh)
        {
            agente.isStopped = true;
        }
        Destroy(gameObject);
    }

    // M�todo para visualizar el rango de audio en el editor
    private void OnDrawGizmosSelected()
    {
        if (audioSource == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, audioMinDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, audioMaxDistance);
    }
}