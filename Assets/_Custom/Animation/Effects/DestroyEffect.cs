using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
