using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuntoSegumiento : MonoBehaviour
{
    public Transform objetivoSeguimiento; // Asigna el objeto de seguimiento en el Inspector
    public Cinemachine.CinemachineVirtualCamera cinemachineCamera; // Asigna la cámara de Cinemachine

    private void Update()
    {
        // Seguir la posición del objetivo
        transform.position = objetivoSeguimiento.position;

        // Alinear horizontalmente con la cámara
        Vector3 direccionHorizontal = cinemachineCamera.transform.forward;
        direccionHorizontal.y = 0; // Remover la componente vertical
        transform.forward = direccionHorizontal.normalized;
    }
}
