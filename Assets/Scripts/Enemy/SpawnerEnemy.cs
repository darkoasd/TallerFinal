using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerEnemy : MonoBehaviour
{
    [System.Serializable]  // Hace que la estructura sea editable desde el inspector de Unity
    public struct SpawnConfig
    {
        public GameObject enemyPrefab;  // El prefab del enemigo
        public Transform spawnPoint;    // Punto de spawn del enemigo
    }
    [SerializeField]  // Hace que la lista sea editable desde el inspector
    private List<SpawnConfig> spawnConfigs;
    public float spawnDelay = 5f;
    private bool spawnTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !spawnTriggered)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(spawnDelay);
        foreach (var config in spawnConfigs)
        {
            Instantiate(config.enemyPrefab, config.spawnPoint.position, config.spawnPoint.rotation);
        }
        spawnTriggered = true; // Previene más spawns, quitar o ajustar según necesidad
    }

    public void ResetSpawner()
    {
        spawnTriggered = false;
    }
}