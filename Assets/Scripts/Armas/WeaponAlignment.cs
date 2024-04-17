using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponAlignment : MonoBehaviour
{
    public Transform cinemachineCamera;

  

    void Update()
    {
        transform.rotation = cinemachineCamera.transform.rotation;
        transform.position = cinemachineCamera.transform.position;
    }
}
