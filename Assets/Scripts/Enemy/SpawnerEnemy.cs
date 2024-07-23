using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
            Vector3 spawnPosition = config.spawnPoint.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                var enemy = Instantiate(config.enemyPrefab, spawnPosition, config.spawnPoint.rotation);
                var navMeshAgent = enemy.GetComponent<NavMeshAgent>();
                if (navMeshAgent != null && !navMeshAgent.isOnNavMesh)
                {
                    Debug.LogError("Failed to place enemy on NavMesh");
                }
            }
            else
            {
                Debug.LogError("Spawn point is not close enough to NavMesh");
            }
        }
        spawnTriggered = true; // Previene más spawns, quitar o ajustar según necesidad
    }

    public void ResetSpawner()
    {
        spawnTriggered = false;
    }
}