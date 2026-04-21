using UnityEngine;
using Fusion;

namespace CombatSystem
{
    public class PropHealth : Health
    {
        public override void Spawned()
        {
            base.Spawned();

            audioHit.SetCharacterType(characterType);
            oldHealth = maxHealth;
            CurrentHealth = maxHealth;

            OnHealthChanged?.Invoke(CurrentHealth);
        }

        protected override void Die()
        {
            GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false); // Hide the prop's mesh
            GetComponent<Collider>().enabled = false; // Disable the collider to prevent further interactions
            ParticleSystem particles = GetComponent<ParticleSystem>();
            if (particles != null)
                particles.Play(); // Play destruction particles if available

            base.Die();
        }
    }
}
