using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyFromGround : MonoBehaviour
{
    public event System.Action PlayerEnterTrigger; // Definición correcta del evento

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEnterTrigger?.Invoke(); // Disparar el evento
        }
    }
}
