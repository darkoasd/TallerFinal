using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public string requiredKeyId; // ID de la llave requerida para abrir la puerta
    public bool initiallyOpen = false; // Indica si la puerta debe empezar abierta
    private Animator animator;
    private bool isOpen = false;
    public GameObject textLlave; // Referencia al objeto que muestra el mensaje de necesidad de llave

    private void Start()
    {
        animator = GetComponent<Animator>();
        isOpen = initiallyOpen; // Establece el estado inicial basado en la variable initiallyOpen
        animator.SetBool("Abierta", isOpen); // Configura el animator con el estado inicial
        ToggleChildCollider(isOpen); // Ajusta el collider del hijo según el estado inicial
    }

    public void ToggleDoor(KeyInventory inventory)
    {
        if ((string.IsNullOrEmpty(requiredKeyId) || (inventory != null && inventory.HasKey(requiredKeyId))) && !isOpen)
        {
            isOpen = true;
            animator.SetBool("Abierta", isOpen);
            ToggleChildCollider(isOpen);
        }
        else if (isOpen)
        {
            isOpen = false;
            animator.SetBool("Abierta", isOpen);
            ToggleChildCollider(isOpen);
        }
        else if (!string.IsNullOrEmpty(requiredKeyId) && (inventory == null || !inventory.HasKey(requiredKeyId)))
        {
           
            StartCoroutine(ShowMessage());
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

    private void ToggleChildCollider(bool state)
    {
        if (transform.childCount > 0)
        {
            Collider childCollider = transform.GetChild(0).GetComponent<Collider>();
            if (childCollider != null)
            {
                childCollider.enabled = !state;
            }
        }
    }
}
