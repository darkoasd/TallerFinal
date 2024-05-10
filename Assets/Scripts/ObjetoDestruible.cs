using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoDestruible : MonoBehaviour
{
    public float health = 50f;
    public List<GameObject> itemPrefabs; // Lista de prefabs de ítems que pueden ser arrojados
    public float dropProbability = 0.5f;

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        Destroy(gameObject);
        if (Random.value <= dropProbability && itemPrefabs.Count > 0)
        {
            // Seleccionar un ítem aleatorio de la lista
            GameObject itemToDrop = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }
}

