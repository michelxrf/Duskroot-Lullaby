using UnityEngine;
using Fusion;
using CombatSystem;


/// <summary>
/// Make objects that heal the player when they interact with them. The item will be despawned after healing the player.
/// </summary>
public class HealingItem : NetworkBehaviour
{
    [SerializeField] private int healAmount = 20;
    [SerializeField] private GameObject interactionTooltip;

    private int playersInRange;
    private bool hasSpawned = false;

    public override void Spawned()
    {
        base.Spawned();
        hasSpawned = true;
        playersInRange = 0;
        if (interactionTooltip != null)
            interactionTooltip.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange++;
        other.GetComponent<PlayerInteractor>()?.EnteredHealingItemArea(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange--;
        other.GetComponent<PlayerInteractor>()?.LeftHealingItemArea(this);
    }

    private void Update()
    {
        UpdateTooltip();
    }

    private void UpdateTooltip()
    {
        if (!hasSpawned || interactionTooltip == null)
            return;

        interactionTooltip.SetActive(playersInRange >= 1);
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
