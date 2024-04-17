using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FearEffects : MonoBehaviour
{
    public Volume volume;
    private Vignette vignette;

    void Start()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            this.vignette = vignette;
        }
    }

    void Update()
    {
        float fearLevel = FindObjectOfType<PlayerController>().nivelDeMiedo; // Optimize this call
        UpdateVignette(fearLevel);
    }

    void UpdateVignette(float fearLevel)
    {
        float intensity = Mathf.Clamp(fearLevel, 0, 1);
        vignette.intensity.value = intensity;
    }
}
