using UnityEngine;
using Fusion;
using CombatSystem;


/// <summary>
/// Make objects that heal the player when they collide with them. The item will be despawned after healing the player.
/// </summary>
public class HealingItem : NetworkBehaviour
{
    [SerializeField] private int healAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if( !HasStateAuthority)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        
        if (health.IsDead())
            return;

        if(health.CurrentHealth < CharacterDataManager.Instance.GetCurrentPlayerCharacter().health)
        {
            AudioUI.instance.PlayUIFood();
            health.RPC_Heal(healAmount);
            Runner.Despawn(Object);
        }
    }
}
