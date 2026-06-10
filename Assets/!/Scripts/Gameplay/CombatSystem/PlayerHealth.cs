using CombatSystem;
using Fusion;
using UnityEngine;

namespace CombatSystem
{
    public class PlayerHealth : Health
    {
        [Header("References")]
        [SerializeField] GameObject playerVisual;
        private PlayerPostProcessController postProcessController;

        MatchStateManager matchStateManager;

        public override void Spawned()
        {
            base.Spawned();
            if (Object.HasInputAuthority)
            {
                postProcessController = FindFirstObjectByType<PlayerPostProcessController>();
            }
            postProcessController?.SetDead(false);

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
            animator?.SetTrigger("Die");
            postProcessController?.SetDead(true);
            Knockback knockback = GetComponent<Knockback>();
            if (knockback != null)
                knockback.enabled = false;

            GetComponent<PlayerSetup>().EnablePlayerControls(false);
            OnDied?.Invoke();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Revive()
        {
            RPC_PlayReviveVisuals();
            CurrentHealth = maxHealth / 4;
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void RPC_PlayReviveVisuals()
        {
            animator?.ResetTrigger("Die");
            animator?.SetTrigger("Revive");

            postProcessController?.SetDead(false);

            GetComponent<PlayerSetup>().EnablePlayerControls(true);
            GetComponent<PlayerSetup>().RPC_EnablePlayerVisuals(true);
        }

        public override void Render()
        {
            base.Render();
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

        public void FinishedDeathAnimation()
        {
            if (HasStateAuthority)
            {
                GetComponent<PlayerSetup>().RPC_EnablePlayerVisuals(false);
                if (matchStateManager == null)
                    matchStateManager = FindFirstObjectByType<MatchStateManager>();

                if (HasStateAuthority && matchStateManager != null)
                    matchStateManager.NotifyPlayerDeath(this, transform.position);
            }
        }
    }
}