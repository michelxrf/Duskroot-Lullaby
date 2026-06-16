using CombatSystem;
using Fusion;
using TMPro;
using UnityEngine;

/// <summary>
/// Automatically emits a floating damage bubble above the character whenever the health changes (e.g., when taking damage).
/// </summary>
[RequireComponent(typeof(Health))]
public class FloatingDamageEmmiter : NetworkBehaviour
{
    Health healthComponent;
    [SerializeField] private GameObject damageBubblePrefab;
    [SerializeField] private GameObject healBubblePrefab;
    [SerializeField] private GameObject xpBubblePrefab;

    public override void Spawned()
    {
        healthComponent = GetComponent<Health>();
        healthComponent.OnReceivedDamage += Emit;

        if (HasStateAuthority && GetComponent<PlayerSetup>())
        {
            CharacterDataManager.Instance.OnExperienceGained += EmitXP;
        }
    }

    //Damage and Healing
    private void Emit(int amount)
    {
        Debug.Log($"{gameObject.name} is emitting {amount} baloon");
        bool isHeal = amount < 0;

        GameObject prefab =
            isHeal
                ? healBubblePrefab
                : damageBubblePrefab;

        if (prefab == null)
            return;

        var bubble = Instantiate(
            prefab,
            transform.position + Vector3.up * 0.5f,
            Quaternion.identity);

        TMP_Text text =
            bubble.GetComponentInChildren<TMP_Text>();

        if (text != null)
        {
            int value = Mathf.Abs(amount);

            text.text = isHeal
                ? $"+{value}"
                : value.ToString();
        }
    }

    //XP
    public void EmitXP(int amount)
    {
        RPC_EmitXP(amount);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EmitXP(int amount)
    {
        if (!(amount > 0))
            return;

        if (xpBubblePrefab == null)
            return;

        if(GetComponent<PlayerSetup>() == null)
            return;

        var bubble = Instantiate(
            xpBubblePrefab,
            transform.position + Vector3.up * 0.5f,
            Quaternion.identity);

        TMP_Text text = bubble.GetComponentInChildren<TMP_Text>();

        if (text != null)
        {
            text.text = $"+{amount} XP";
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasStateAuthority)
    {
        if (healthComponent != null)
            healthComponent.OnReceivedDamage -= Emit;

        if (CharacterDataManager.Instance != null && HasStateAuthority && GetComponent<PlayerSetup>())
        {
            CharacterDataManager.Instance.OnExperienceGained -= EmitXP;
        }
    }
}
