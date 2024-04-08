using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistola : Arma
{
    public GameObject bulletHolePrefab;
    public LayerMask ignoreLayers;
    public override void Disparar()
    {
        if (municionEnCargador > 0 && Time.time - tiempoUltimoDisparo >= tiempoEntreDisparos)
        {
            tiempoUltimoDisparo = Time.time;
            municionEnCargador--;

            RaycastHit hit;
            Vector3 rayOrigin = Camera.main.transform.position;
            Vector3 direction = Camera.main.transform.forward;

            // Si la precisi�n es perfecta, dispara sin variaci�n
            if (precisionActual >= 1.0f)
            {
                if (Physics.Raycast(rayOrigin, direction, out hit, rango, ~ignoreLayers))
                {
                    GameObject bulletHoleInstance = null;
                    if (bulletHolePrefab != null)
                    {
                        bulletHoleInstance = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                    }

                    // Comprobamos si el objeto golpeado tiene el componente SaludEnemigo
                    Enemy saludEnemigo = hit.collider.GetComponent<Enemy>();
                    if (saludEnemigo != null)
                    {
                        saludEnemigo.RecibirDa�o(da�o);
                    }

                    // Destruye el bullet hole despu�s de 3 segundos
                    if (bulletHoleInstance != null)
                    {
                        Destroy(bulletHoleInstance, 3.0f); // Destruye el bullet hole despu�s de 3 segundos
                    }
                }               

            }
            else // Manejar la precisi�n no perfecta
            {
                // Aqu� necesitas ajustar la direcci�n basada en la precisi�nActual
                // Esto se hace aplicando una peque�a variaci�n a la direcci�n del disparo
                float variacionMaxima = (1.0f - precisionActual) * 5.0f; // Ajusta este valor seg�n sea necesario
                Vector3 variacion = new Vector3(Random.Range(-variacionMaxima, variacionMaxima), Random.Range(-variacionMaxima, variacionMaxima), 0);
                Vector3 direccionVariada = Quaternion.Euler(variacion) * direction;

                if (Physics.Raycast(rayOrigin, direccionVariada, out hit, rango, ~ignoreLayers))
                {
                    // El resto del c�digo es similar al manejo de precisi�n perfecta
                    if (bulletHolePrefab != null)
                    {
                        Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                       
                    }
                    Enemy saludEnemigo = hit.collider.GetComponent<Enemy>();
                    if (saludEnemigo != null)
                    {
                        saludEnemigo.RecibirDa�o(da�o);
                    }
                }
            }
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
