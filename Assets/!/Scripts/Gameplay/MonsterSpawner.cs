using UnityEngine;
using Fusion;
using CombatSystem;

public class MonsterSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform rallypoint;
    [SerializeField] private float spawnCooldown;
    [SerializeField] private float maxPopulation;

    int currentPopulation = 0;
    float spawnCooldownTimer = 0f;

    public override void Spawned()
    {
        GetComponent<PropHealth>().OnDied += () => this.enabled = false;
        if(spawnPoint == null)
            spawnPoint = transform; // Default to spawner's position if no spawn point is set
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;
        
        if (currentPopulation < maxPopulation)
        {
            spawnCooldownTimer -= Runner.DeltaTime;
            if (spawnCooldownTimer <= 0f)
            {
                SpawnMonster();
                spawnCooldownTimer = spawnCooldown; // Reset cooldown after spawning
            }
        }
        
    }

    void SpawnMonster()
    {
        if (monsterPrefab != null && spawnPoint != null)
        {
            var newMonster = Runner.Spawn(monsterPrefab, spawnPoint.position, spawnPoint.rotation);

            if(rallypoint != null)
                newMonster.transform.SetParent(rallypoint);

            newMonster.GetComponent<EnemyHealth>().OnDied += () => currentPopulation--; // Decrease population when monster dies
            currentPopulation++;
        }
    }
}