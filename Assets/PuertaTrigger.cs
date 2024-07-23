using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaTrigger : MonoBehaviour
{
    public PuertaAbiertaPuzzle puerta;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(puerta.playerTag))
        {
            puerta.SetPlayerCerca(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(puerta.playerTag))
        {
            puerta.SetPlayerCerca(false);
        }
    }
}