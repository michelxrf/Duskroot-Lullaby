using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System;
using UnityEngine.AI;
using ProgressionSystem;


namespace CombatSystem
{
    /// <summary>
    /// Keeps track of a character's heath and handles damage and death.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioHitNotifier))]
    public class Health : NetworkBehaviour
    {
        [SerializeField] protected int armor = 0;
        [SerializeField] protected int maxHealth = 100;
        [SerializeField] protected float destroyCorpseAfterSeconds = 20f;

        [Networked, OnChangedRender(nameof(OnHealthChangedRender))]
        [HideInInspector] public int CurrentHealth { get; protected set; }
        protected int oldHealth { get; set; }

        [Header("Audio")]
        protected AudioHitNotifier audioHit;
        [SerializeField] protected string characterType;

        public Action<int> OnHealthChanged;
        public Action OnHit;
        public Action OnDied;

        protected Animator animator;


        public override void Spawned()
        {
            animator = GetComponentInChildren<Animator>();
            audioHit = GetComponent<AudioHitNotifier>();

            if (!HasStateAuthority)
                return;
        }

        /// <summary>
        /// Reduces object's health and triggers death if health reaches zero.
        /// </summary>
        /// <param name="damage"></param>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_TakeDamage(int damage, string weaponType)
        {
            if (damage < 0)
            {
                damage = 0;
                Debug.LogWarning("Damage cannot be negative. Setting damage to 0. Use Heal() to increase health.");
            }

            CurrentHealth = Mathf.Clamp(CurrentHealth - (damage * (100 - armor)/100), 0, maxHealth);
            audioHit.NotifyHit(weaponType);
            OnHit?.Invoke();
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        void OnHealthChangedRender()
        {
            if (CurrentHealth <= 0)
            {
                Die();
            }
            else if (CurrentHealth > oldHealth)
            {
                // Health increased 
            }
            else if (CurrentHealth < oldHealth)
            {
                // Health decreased
                animator?.SetTrigger("Hit");
            }

            oldHealth = CurrentHealth;
        }

        /// <summary>
        /// Processes the object's death.
        /// </summary>
        protected virtual void Die()
        {
            animator?.SetTrigger("Die");
            Knockback knockback = GetComponent<Knockback>();
            if (knockback != null)
                knockback.enabled = false;

            if (HasStateAuthority)
                StartCoroutine(DestroyAfterDeathAnimation());

            RPC_ApplyRewards();
            OnDied?.Invoke();
        }

        /// <summary>
        /// Used to heal character
        /// </summary>
        /// <param name="amount"></param>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Heal(int amount)
        {
            if (IsDead())
                return;

            if (amount < 0)
            {
                amount = 0;
                Debug.LogWarning("Heal amount cannot be negative. Setting heal amount to 0.");
            }

            animator?.SetTrigger("Heal");

            maxHealth = CharacterDataManager.Instance.GetCurrentPlayerCharacter().health;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth);
        }


        /// <summary>
        /// TODO: move out of health
        /// </summary>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        void RPC_ApplyRewards()
        {
            GetComponent<Reward>()?.ApplyReward();
        }

        IEnumerator DestroyAfterDeathAnimation()
        {
            // Wait for the death animation to finish before destroying the object
            if(animator != null)
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + destroyCorpseAfterSeconds);
            else
                yield return new WaitForSeconds(destroyCorpseAfterSeconds);
            Runner.Despawn(Object);
        }

        public bool IsDead()
        {
            return CurrentHealth <= 0;
        }
    }
}


