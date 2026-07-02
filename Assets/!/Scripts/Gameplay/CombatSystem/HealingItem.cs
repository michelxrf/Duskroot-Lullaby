using UnityEngine;
using Fusion;
using CombatSystem;


/// <summary>
/// Make objects that heal the player when they interact with them. The item will be despawned after healing the player.
/// </summary>
public class HealingItem : NetworkBehaviour
{
    [SerializeField] private int healAmount = 20;
    [SerializeField] private Canvas interactionTooltip;

    private bool hasSpawned = false;

    public override void Spawned()
    {
        base.Spawned();

        if (interactionTooltip != null)
            interactionTooltip.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        interactionTooltip.enabled = true;
        other.GetComponent<PlayerInteractor>()?.EnteredHealingItemArea(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerSetup player = other.GetComponent<PlayerSetup>();
        if (player == null || !player.IsLocalPlayer())
            return;

        interactionTooltip.enabled = false;
        other.GetComponent<PlayerInteractor>()?.LeftHealingItemArea(this);
    }

    public void Consume(PlayerHealth health)
    {
        if (health.IsDead())
            return;

        if (health.CurrentHealth < CharacterDataManager.Instance.GetCurrentPlayerCharacter().health)
        {
            AudioUI.instance.PlayUIFood();
            RPC_Interact(health);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Interact(PlayerHealth health)
    {
        health.RPC_Heal(healAmount);
        Runner.Despawn(Object);
    }
}
