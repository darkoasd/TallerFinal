using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunicionItem : MonoBehaviour
{
    public int cantidadMunicion = 10;  // Cantidad de munición que proporciona este item
    private bool jugadorEnContacto = false;  // Indica si el jugador está en contacto con el objeto de munición

    private void Update()
    {
        if (jugadorEnContacto && Input.GetKeyDown(KeyCode.E))
        {
            Pistola arma = GameObject.FindWithTag("Player").GetComponentInChildren<Pistola>();
            if (arma != null)
            {
                arma.IncrementarMunicionDeReserva(cantidadMunicion);
                Destroy(gameObject);  // Destruye el objeto de munición después de recogerlo
            }
            Escopeta escopeta = GameObject.FindWithTag("Player").GetComponentInChildren<Escopeta>();
            if (escopeta != null)
            {
                escopeta.IncrementarMunicionDeReserva(cantidadMunicion);
                Destroy(gameObject);  // Destruye el objeto de munición después de recogerlo
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnContacto = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnContacto = false;
        }
    }
}