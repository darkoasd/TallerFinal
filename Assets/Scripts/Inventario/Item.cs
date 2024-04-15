using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite icon;
    // Aquí puedes agregar más propiedades comunes, como durabilidad, cantidad, etc.

    // Método para usar el item
    public virtual void Use()
    {
        // Implementa cómo se usa este item
        Debug.Log("Using " + itemName);
    }
}
