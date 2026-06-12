using UnityEngine;
using Fusion;
using FMOD.Studio;
using FMODUnity;

namespace CombatSystem
{
    public class PropHealth : Health
    {
        [Header("FMOD")]
        [SerializeField] private EventReference damageEvent;
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
            PlayDamage();
            base.Die();
        }

        public void PlayDamage()
        {
            EventInstance punchInstance = RuntimeManager.CreateInstance(damageEvent);
            punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            punchInstance.start();
            punchInstance.release();
        }
    }
}
