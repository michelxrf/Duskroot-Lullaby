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
        [SerializeField] int armor = 0;
        [SerializeField] int maxHealth = 100;
        [SerializeField] float destroyCorpseAfterSeconds = 20f;

        [Networked, OnChangedRender(nameof(OnHealthChangedRender))]
        [HideInInspector] public int CurrentHealth { get; private set; }
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
                // player Init
                if (GetComponent<PlayerAttack>() != null)
                {
                    maxHealth = CharacterDataManager.Instance.GetCurrentPlayerCharacter().health;
                    characterType = CharacterDataManager.Instance.GetCurrentPlayerCharacter().characterId;
                }

                // enemy Init
                if (GetComponent<EnemySetup>() != null)
                {
                    if(GetComponent<EnemySetup>().IsInitialized())
                    {
                        maxHealth = GetComponent<EnemySetup>().GetEnemyData().health;
                        characterType = GetComponent<EnemySetup>().GetEnemyData().CharacterId;
                    }
                    else
                    {
                        GetComponent<EnemySetup>().OnInit += () =>
                        {
                            maxHealth = GetComponent<EnemySetup>().GetEnemyData().health;
                            characterType = GetComponent<EnemySetup>().GetEnemyData().CharacterId;
                        };
                    }
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
            CurrentHealth = Mathf.Clamp(CurrentHealth - (damage * (100 - armor)/100), 0, maxHealth);
            audioHit.NotifyHit(weaponType);
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
            gameObject.DisableAllComponents<CharacterLook>();

            // Disable AI controls
            gameObject.DisableAllComponents<StateMachine>();

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if(agent != null)
                agent.isStopped = true;            

            RPC_AddExperience();
            animator.SetTrigger("Die");

            if(HasStateAuthority)
                StartCoroutine(DestroyAfterDeathAnimation());
        }


        /// <summary>
        /// TODO: move out of health
        /// </summary>
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

        public bool IsDead()
        {
            return CurrentHealth <= 0;
        }
    }
}


