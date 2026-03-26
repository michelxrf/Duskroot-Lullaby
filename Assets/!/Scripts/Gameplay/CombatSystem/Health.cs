using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System;
using UnityEngine.AI;


namespace CombatSystem
{
    /// <summary>
    /// Keeps track of a character's heath and handles damage and death.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioHitNotifier))]
    public class Health : NetworkBehaviour
    {
        [SerializeField] int maxHealth = 100;
        [SerializeField] float destroyCorpseAfterSeconds = 20f;

        [Networked, OnChangedRender(nameof(OnHealthChangedRender))]
        public int CurrentHealth { get; private set; }
        int oldHealth { get; set; }

        [Header("Audio")]
        AudioHitNotifier audioHit;
        [SerializeField] string characterType;

        public Action<int> OnHealthChanged;

        Animator animator;


        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                // TODO: Separate player and enemy health initialization.
                if (GetComponent<PlayerAttack>() != null)
                {
                    maxHealth = CharacterDataManager.Instance.GetCurrentPlayerCharacter().health;
                    characterType = CharacterDataManager.Instance.GetCurrentPlayerCharacter().characterId;
                }

                oldHealth = maxHealth;
                CurrentHealth = maxHealth;
            }

            animator = GetComponentInChildren<Animator>();

            audioHit = GetComponent<AudioHitNotifier>();
            audioHit.SetCharacterType(characterType);
        }

        /// <summary>
        /// Reduces object's health and triggers death if health reaches zero.
        /// </summary>
        /// <param name="damage"></param>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_TakeDamage(int damage, string weaponType)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - (damage), 0, maxHealth);
            audioHit.NotifyHit(weaponType);
            Debug.Log($"{gameObject.name} took {oldHealth - CurrentHealth} damage. Current health: {CurrentHealth}/{maxHealth}");
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
                animator.SetTrigger("Hit");
            }

            oldHealth = CurrentHealth;
        }

        /// <summary>
        /// Processes the object's death.
        /// </summary>
        void Die()
        {
            // disable character     
            gameObject.DisableAllComponents<Collider>();
            gameObject.DisableComponent<SimpleKCC>();

            // Disbable player controls
            gameObject.DisableComponent<PlayerAttack>();
            gameObject.DisableComponent<PlayerMovement>();

            // Disable AI controls
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if(agent != null)
                agent.isStopped = true;
            
            gameObject.DisableAllComponents<StateMachine>();

            RPC_AddExperience();
            animator.SetTrigger("Die");

            if(HasStateAuthority)
                StartCoroutine(DestroyAfterDeathAnimation());
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        void RPC_AddExperience()
        {
            GetComponent<Reward>()?.ApplyReward();
        }

        IEnumerator DestroyAfterDeathAnimation()
        {
            // Wait for the death animation to finish before destroying the object
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + destroyCorpseAfterSeconds);
            Runner.Despawn(Object);
        }

    }
}


