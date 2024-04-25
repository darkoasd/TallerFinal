using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoDestruible : MonoBehaviour
{
    public float health = 50f;
    public GameObject itemPrefab;
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
        if (Random.value <= dropProbability)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}

