using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pistola : Arma
{
    public GameObject bulletHolePrefab;
    public LayerMask ignoreLayers;
    public float bulletHoleLifetime = 5f;
    public TextMeshProUGUI textoMunicion;

    public override void Disparar()
    {
        if (GameManager.instance.IsInventoryOpen())
        {
            return;
        }
        if (municionEnCargador > 0 && Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
        {
            animator.SetBool("Disparando", true);
            StartCoroutine(TiempoDisparo());

            tiempoUltimoDisparo = Time.time;
            municionEnCargador--;
            ActualizarTextoMunicion();
            AplicarRecoil();
            Vector3 direction = CalcularDireccionDisparo();

            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, direction, out hit, rango, ~ignoreLayers))
            {
                if (bulletHolePrefab != null && hit.collider.GetComponent<Enemy>() == null)
                {
                    GameObject createdBulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                    Destroy(createdBulletHole, bulletHoleLifetime);
                }

                Enemy saludEnemigo = hit.collider.GetComponentInParent<Enemy>();
                if (saludEnemigo != null)
                {
                    string parteDelCuerpo = hit.collider.CompareTag("Cabeza") ? "Cabeza" : "Cuerpo";
                    saludEnemigo.RecibirDa�o(da�o, cameraTransform.position, parteDelCuerpo);
                }
                else
                {
                    Debug.LogWarning("saludEnemigo es null. Aseg�rate de que el enemigo tenga el componente Enemy.");
                }

                ObjetoDestruible destructibleTarget = hit.collider.GetComponent<ObjetoDestruible>();
                if (destructibleTarget != null)
                {
                    destructibleTarget.ReceiveDamage(da�o);
                }
            }
        }
        else if (municionEnCargador <= 0)
        {
            Debug.Log("Sin munici�n, presione 'R' para recargar.");
        }
    }

    public void IncrementarMunicionDeReserva(int cantidad)
    {
        municionDeReserva += cantidad;
        ActualizarTextoMunicion();
    }

    IEnumerator TiempoDisparo()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Disparando", false);
    }

    protected override void Update()
    {
        base.Update();

        if (!GameManager.instance.IsInventoryOpen())
        {
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

    public void Recargar()
    {
        if (municionDeReserva > 0 && municionEnCargador < capacidadCargador)
        {
            int municionParaRecargar = capacidadCargador - municionEnCargador;
            int municionARecargar = Mathf.Min(municionParaRecargar, municionDeReserva);

            municionEnCargador += municionARecargar;
            municionDeReserva -= municionARecargar;
            ActualizarTextoMunicion();
        }
    }

    public void ActualizarTextoMunicion()
    {
        if (textoMunicion != null)
            textoMunicion.text = $"Munici�n: {municionEnCargador} / {municionDeReserva}";
    }

    private void OnEnable()
    {
        if (textoMunicion != null)
            textoMunicion.gameObject.SetActive(true);
        ActualizarTextoMunicion();

        animator.SetBool("ConPistola", true);
        animator.SetBool("ConEscopeta", false);
    }

    private void OnDisable()
    {
        if (textoMunicion != null)
            textoMunicion.gameObject.SetActive(false);

        animator.SetBool("ConPistola", false);
    }
}