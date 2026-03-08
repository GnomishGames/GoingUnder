using System.Collections;
using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;

    public void SpawnRandomPrefab()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogError("RandomPrefabSpawner: No prefabs assigned!");
            return;
        }

        StartCoroutine(SpawnAfterFrame());
    }

    IEnumerator SpawnAfterFrame()
    {
        // Wait until end of frame so all Awake methods complete first
        yield return new WaitForEndOfFrame();

        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        if (selectedPrefab != null)
        {
            Instantiate(selectedPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning($"RandomPrefabSpawner: Prefab at index {randomIndex} is null!");
        }
    }
}
