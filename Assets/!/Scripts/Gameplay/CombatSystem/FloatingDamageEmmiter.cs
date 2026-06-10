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
        CharacterDataManager.Instance.OnExperienceGained += EmitXP;
    }

    //Damage and Healing
    private void Emit(int amount)
    {
        RPC_Emit(amount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_Emit(int amount)
    {
        bool isHeal = amount < 0;

        GameObject prefab =
            isHeal
                ? healBubblePrefab
                : damageBubblePrefab;

        if (prefab == null)
            return;

        var bubble = Runner.Spawn(
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_EmitXP(int amount)
    {
        if (xpBubblePrefab == null)
            return;

        var bubble = Runner.Spawn(
            xpBubblePrefab,
            transform.position + Vector3.up * 0.5f,
            Quaternion.identity);

        TMP_Text text = bubble.GetComponentInChildren<TMP_Text>();

        if (text != null)
        {
            text.text = $"+{amount} XP";
        }
    }

    private void OnDestroy()
    {
        if (healthComponent != null)
            healthComponent.OnReceivedDamage -= Emit;

        if (CharacterDataManager.Instance != null)
            CharacterDataManager.Instance.OnExperienceGained -= EmitXP;
    }
}
