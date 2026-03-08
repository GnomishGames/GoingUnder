using System.Collections;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    void Awake()
    {
        //Instantiate(prefabToSpawn, transform.position, transform.rotation);
        StartCoroutine(SpawnAfterFrame());
    }

    IEnumerator SpawnAfterFrame()
    {
        // Wait until end of frame so all Awake methods complete first
        yield return new WaitForEndOfFrame();

        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}