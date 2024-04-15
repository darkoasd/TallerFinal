using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite icon;
    // Aqu� puedes agregar m�s propiedades comunes, como durabilidad, cantidad, etc.

    // M�todo para usar el item
    public virtual void Use()
    {
        // Implementa c�mo se usa este item
        Debug.Log("Using " + itemName);
    }
}
