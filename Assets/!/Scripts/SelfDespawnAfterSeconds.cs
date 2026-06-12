using Fusion;
using UnityEngine;

public class SelfDespawnAfterSeconds : MonoBehaviour
{
    [SerializeField] private float secondsToDespawn = 1f;

    private void Start()
    {
        Destroy(gameObject, secondsToDespawn);
    }
}
