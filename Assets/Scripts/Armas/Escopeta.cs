using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escopeta : Arma
{
    public GameObject bulletHolePrefab;  // Prefab de Sprite para marcar donde impactan los disparos
    public LayerMask ignoreLayers;       // Capas que ignorará al disparar
    public int numBalas = 8;             // Número de balas disparadas en un solo tiro
    public float spreadAngle = 15f;      // Ángulo de dispersión de las balas
    public LayerMask groundLayer;        // Máscara de capa específica para el "suelo"

    public override void Disparar()
    {

        if (municionEnCargador > 0 && Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
        {
            tiempoUltimoDisparo = Time.time;
            municionEnCargador--;

            for (int i = 0; i < numBalas; i++)
            {
                Vector3 spread = new Vector3(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0
                );

                Vector3 direction = Quaternion.Euler(spread) * Camera.main.transform.forward;
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.transform.position, direction, out hit, rango, ~ignoreLayers))
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        if (bulletHolePrefab != null)
                        {
                            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                            bulletHole.transform.Rotate(0, 0, 0); // Ajustar la rotación del sprite si es necesario

                            // Destruye el bullet hole después de 5 segundos
                            Destroy(bulletHole, 5f);
                        }
                    }

                    Enemy saludEnemigo = hit.collider.GetComponent<Enemy>();
                    if (saludEnemigo != null)
                    {
                        saludEnemigo.RecibirDaño(daño, spread);
                    }
                    ObjetoDestruible destructibleTarget = hit.collider.GetComponent<ObjetoDestruible>();
                    if (destructibleTarget != null)
                    {
                        destructibleTarget.ReceiveDamage(daño);
                    }
                }
            }
        }
        else if (municionEnCargador <= 0)
        {
            Debug.Log("Sin munición, presione 'R' para recargar.");  // Recordatorio para recargar
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
