using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public ScaryEnemy scaryEnemy; // Referencia al enemigo que debe esconderse

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && scaryEnemy != null)
        {
            scaryEnemy.TriggerHide();
        }
    }
}
