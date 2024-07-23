using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaAbiertaPuzzle : MonoBehaviour
{
    public Transform puntoInicio;
    public Transform puntoFinal;
    public float tiempoDeApertura = 10f;
    public GameObject[] enemigos;
    public Transform[] puntosDeSpawn;
    public float intervaloDeSpawn = 2f;
    public int maximoEnemigos = 10;
    public string playerTag = "Player";
    public string requiredKeyId;
    public GameObject textLlave;

    private bool abriendo = false;
    private float tiempoTranscurrido = 0f;
    private bool playerCerca = false;
    private int enemigosGenerados = 0;

    public KeyInventory playerInventory;

    private void Start()
    {
        // Configuración inicial si es necesario
    }

    private void Update()
    {
        if (playerCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (playerInventory != null)
            {
                Debug.Log("PlayerInventory is not null.");
                if (playerInventory.HasKey(requiredKeyId))
                {
                    Debug.Log("Player has the key: " + requiredKeyId);
                    AbrirPuerta();
                }
                else
                {
                    Debug.Log("Player does not have the key: " + requiredKeyId);
                    StartCoroutine(ShowMessage());
                }
            }
            else
            {
                Debug.Log("PlayerInventory is null.");
            }
        }
    }

    public void SetPlayerCerca(bool estado)
    {
        playerCerca = estado;
    }

    public void SetPlayerInventory(KeyInventory inventory)
    {
        playerInventory = inventory;
        Debug.Log("PlayerInventory set: " + (playerInventory != null));
    }

    public void AbrirPuerta()
    {
        if (!abriendo)
        {
            StartCoroutine(AbrirPuertaCoroutine());
            StartCoroutine(SpawnEnemigosCoroutine());
        }
    }

    private IEnumerator AbrirPuertaCoroutine()
    {
        abriendo = true;
        tiempoTranscurrido = 0f;
        enemigosGenerados = 0;

        Vector3 posicionInicial = puntoInicio.position;
        Vector3 posicionFinal = puntoFinal.position;

        while (tiempoTranscurrido < tiempoDeApertura)
        {
            tiempoTranscurrido += Time.deltaTime;
            float porcentaje = tiempoTranscurrido / tiempoDeApertura;
            transform.position = Vector3.Lerp(posicionInicial, posicionFinal, porcentaje);
            yield return null;
        }

        transform.position = puntoFinal.position;
        abriendo = false;
    }

    private IEnumerator SpawnEnemigosCoroutine()
    {
        while (abriendo && enemigosGenerados < maximoEnemigos)
        {
            foreach (Transform puntoDeSpawn in puntosDeSpawn)
            {
                if (enemigosGenerados >= maximoEnemigos)
                {
                    break;
                }

                SpawnEnemigo(puntoDeSpawn.position);
                yield return new WaitForSeconds(intervaloDeSpawn);
            }
        }
    }

    private void SpawnEnemigo(Vector3 posicion)
    {
        if (enemigos.Length > 0)
        {
            int indiceEnemigo = Random.Range(0, enemigos.Length);
            Instantiate(enemigos[indiceEnemigo], posicion, Quaternion.identity);
            enemigosGenerados++;
        }
    }

    private IEnumerator ShowMessage()
    {
        if (textLlave != null)
        {
            textLlave.SetActive(true);
            yield return new WaitForSeconds(3);
            textLlave.SetActive(false);
        }
    }
}