using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Escopeta : Arma
{
    public GameObject bulletHolePrefab;  // Prefab de Sprite para marcar donde impactan los disparos
    public LayerMask ignoreLayers;       // Capas que ignorar� al disparar
    public int numBalas = 8;             // N�mero de balas disparadas en un solo tiro
    public float spreadAngle = 15f;      // �ngulo de dispersi�n de las balas
    public LayerMask groundLayer;        // M�scara de capa espec�fica para el "suelo"
    public TextMeshProUGUI textoMunicion;

    protected override void Start()
    {
        base.Start();
        ActualizarTextoMunicion();
    }

    public override void Disparar()
    {
        if (municionEnCargador > 0 && Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
        {
            tiempoUltimoDisparo = Time.time;
            municionEnCargador--;
            ActualizarTextoMunicion();
            animator.SetBool("DisparandoEscopeta", true);

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
                            bulletHole.transform.Rotate(0, 0, 0); // Ajustar la rotaci�n del sprite si es necesario

                            // Destruye el bullet hole despu�s de 5 segundos
                            Destroy(bulletHole, 5f);
                        }
                    }

                    Enemy saludEnemigo = hit.collider.GetComponentInParent<Enemy>();
                    if (saludEnemigo != null)
                    {
                        string parteDelCuerpo = hit.collider.CompareTag("Cabeza") ? "Cabeza" : "Cuerpo";
                        saludEnemigo.RecibirDa�o(da�o, Camera.main.transform.position, parteDelCuerpo);
                    }

                    ObjetoDestruible destructibleTarget = hit.collider.GetComponent<ObjetoDestruible>();
                    if (destructibleTarget != null)
                    {
                        destructibleTarget.ReceiveDamage(da�o);
                    }
                }
            }
            StartCoroutine(ResetDisparandoEstado());
        }
        else if (municionEnCargador <= 0)
        {
            Debug.Log("Sin munici�n, presione 'R' para recargar.");  // Recordatorio para recargar
        }
    }

    public void IncrementarMunicionDeReserva(int cantidad)
    {
        municionDeReserva += cantidad;
        ActualizarTextoMunicion();
    }

    IEnumerator ResetDisparandoEstado()
    {
        yield return new WaitForSeconds(0.1f); // Ajusta este tiempo seg�n la animaci�n de disparo
        animator.SetBool("DisparandoEscopeta", false);
    }

    private void ActualizarTextoMunicion()
    {
        if (textoMunicion != null)
            textoMunicion.text = $"Munici�n: {municionEnCargador} / {municionDeReserva}";
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
        animator.SetBool("ApuntandoEscopeta", Input.GetButton("Fire2"));
    }

    private void OnEnable()
    {
        if (textoMunicion != null)
            textoMunicion.gameObject.SetActive(true);
        ActualizarTextoMunicion();

        animator.SetBool("ConEscopeta", true);
        animator.SetBool("ConPistola", false); // Asegurarte de que la pistola est� desactivada
    }

    private void OnDisable()
    {
        if (textoMunicion != null)
            textoMunicion.gameObject.SetActive(false);
        animator.SetBool("ConEscopeta", false);
    }
}