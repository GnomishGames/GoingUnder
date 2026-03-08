using UnityEngine;

public class SpawnButton : MonoBehaviour
{
    //find the random prefab spawner in the scene and call its spawn method when this button is clicked
    public void Spawn()
    {
        RandomPrefabSpawner spawner = FindFirstObjectByType<RandomPrefabSpawner>();
        if (spawner != null)        {
            spawner.SpawnRandomPrefab();    
        }
        else
        {
            Debug.LogError("SpawnButton: No RandomPrefabSpawner found in the scene!");
        }   
    }
}
