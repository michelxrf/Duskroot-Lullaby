using CombatSystem;
using Fusion;
using UnityEngine;

namespace CombatSystem
{
    public class PlayerHealth : Health
    {
        MatchStateManager matchStateManager;

        public override void Spawned()
        {
            base.Spawned();
            matchStateManager = FindFirstObjectByType<MatchStateManager>();

            // player Init
            if (GetComponent<PlayerAttack>() != null)
            {
                maxHealth = CharacterDataManager.Instance.GetCurrentPlayerCharacter().health;
                characterType = CharacterDataManager.Instance.GetCurrentPlayerCharacter().characterId;

                CharacterDataManager.Instance.OnLevelUp += () => { maxHealth = CharacterDataManager.Instance.GetCurrentPlayerCharacter().health; };
            }

            audioHit.SetCharacterType(characterType);
            oldHealth = maxHealth;
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        protected override void Die()
        {
            PlayerState(false);

            animator?.SetTrigger("Die");
            Knockback knockback = GetComponent<Knockback>();
            if (knockback != null)
                knockback.enabled = false;

            if (matchStateManager != null)
                matchStateManager.NotifyPlayerDeath(this, transform.position);

            OnDied?.Invoke();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Revive()
        {
            animator?.SetTrigger("Revive");
            Knockback knockback = GetComponent<Knockback>();
            if (knockback != null)
                knockback.enabled = true;

            PlayerState(true);
            
            CurrentHealth = maxHealth / 4;
        }

        public PlayerRef GetPlayerRef()
        {
            if (Object == null || !Object.IsValid)
                return PlayerRef.None;

            return Object.InputAuthority;
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (GetInput(out NetworkInputData data))
            {
                if (data.DebugRevive)
                {
                    if (IsDead())
                    {
                        RPC_Revive();
                    }
                }
            }
        }

        private void PlayerState(bool value)
        {
            GetComponent<PlayerAttack>().enabled = value;
            GetComponent<PlayerMovement>().enabled = value;
            GetComponent<CharacterLook>().enabled = value;
        }
    }

}