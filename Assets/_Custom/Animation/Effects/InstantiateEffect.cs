using UnityEngine;

public class InstantiateEffect : MonoBehaviour
{
    public static void PlayEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation)
    {
        if (effectPrefab == null)
        {
            Debug.LogWarning("Effect prefab is null. Cannot play effect.");
            return;
        }
        
        Instantiate(effectPrefab, position, rotation);
    }
}
