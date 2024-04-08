using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energybar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(float maxEnergy)
    {
        slider.maxValue = maxEnergy;
        slider.value = maxEnergy;
    }

    public void SetHealth(float Energy)
    {
        slider.value = Energy;
    }
}
