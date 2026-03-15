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

        RemoveDeadCreatures();

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

    // remove other creatures that are dead
    public void RemoveDeadCreatures()
    {
        CreatureStats[] allCreatures = FindObjectsByType<CreatureStats>(FindObjectsSortMode.None);
        foreach (CreatureStats creature in allCreatures)
        {
            if (creature.isDead)
            {
                // remove creature from initiative turn order if it's in there
                Initiative initiative = FindAnyObjectByType<Initiative>();
                if (initiative != null)                {
                    initiative.RemoveFromTurnOrder(creature.transform);
                }
                Destroy(creature.gameObject);
            }
        }
    }
}
