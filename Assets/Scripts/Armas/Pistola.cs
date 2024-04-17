using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistola : Arma
{
    public GameObject bulletHolePrefab;
    public LayerMask ignoreLayers;
    public override void Disparar()
    {
        // Verifica si hay munición en el cargador y si ha pasado suficiente tiempo desde el último disparo
        if (municionEnCargador > 0 && Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
        {
            tiempoUltimoDisparo = Time.time;  // Actualiza el tiempo del último disparo
            municionEnCargador--;  // Decrementa la munición en el cargador

            RaycastHit hit;
            Vector3 rayOrigin = Camera.main.transform.position;
            Vector3 direction = Camera.main.transform.forward;

            if (Physics.Raycast(rayOrigin, direction, out hit, rango, ~ignoreLayers))
            {
                if (bulletHolePrefab != null)
                {
                    Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                }

                // Aplica daño a enemigos
                Enemy saludEnemigo = hit.collider.GetComponent<Enemy>();
                if (saludEnemigo != null)
                {
                    saludEnemigo.RecibirDaño(daño);
                }

                // Aplica daño a objetos destruibles
                ObjetoDestruible destructibleTarget = hit.collider.GetComponent<ObjetoDestruible>();
                if (destructibleTarget != null)
                {
                    destructibleTarget.DestroyAndReplace();
                }
            }
        }
        else if (municionEnCargador <= 0)
        {
            Debug.Log("Sin munición, presione 'R' para recargar."); // Mensaje de depuración para indicar que no hay munición
        }
    }

    public void Recargar()
    {
        if (municionDeReserva > 0 && municionEnCargador < capacidadCargador)
        {
            int municionParaRecargar = capacidadCargador - municionEnCargador;
            int municionARecargar = Mathf.Min(municionParaRecargar, municionDeReserva);

            municionEnCargador += municionARecargar;
            municionDeReserva -= municionARecargar;
        }
    }


    protected override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Fire1"))
        {
            Disparar();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Recargar();
        }

    }
}
