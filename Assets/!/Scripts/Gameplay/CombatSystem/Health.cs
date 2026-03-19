using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;


namespace CombatSystem
{
    /// <summary>
    /// Keeps track of a character's heath and handles damage and death.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Health : NetworkBehaviour
    {
        [SerializeField] int maxHealth = 100;

        [Networked, OnChangedRender(nameof(OnHealthChanged))]
        int CurrentHealth { get; set; }
        int oldHealth { get; set; }

        Animator animator;


        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                oldHealth = maxHealth;
                CurrentHealth = maxHealth;
            }


            animator = GetComponentInChildren<Animator>();
        }

        /// <summary>
        /// Reduces object's health and triggers death if health reaches zero.
        /// </summary>
        /// <param name="damage"></param>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCTakeDamage(int damage)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - (damage), 0, maxHealth);
        }

        void OnHealthChanged()
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
            Debug.Log("Object has died.");

            // disable character     
            gameObject.DisableAllComponents<Collider>();
            gameObject.DisableComponent<SimpleKCC>();

            // Disbable player controls
            gameObject.DisableComponent<PlayerAttack>();
            gameObject.DisableComponent<PlayerMovement>();

            // Disable AI controls
            // TODO

            animator.SetTrigger("Die");
        }


    }
}


