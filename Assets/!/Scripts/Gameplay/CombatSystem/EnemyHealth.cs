using CombatSystem;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

namespace CombatSystem
{
    public class EnemyHealth : Health
    {
        EnemyFeedbacks feedbacks;
        public override void Spawned()
        {
            base.Spawned();
            feedbacks = GetComponent<EnemyFeedbacks>();

            if (GetComponent<EnemySetup>() != null)
            {
                if (GetComponent<EnemySetup>().IsInitialized())
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
                        audioHit.SetCharacterType(characterType);
                        oldHealth = maxHealth;
                        CurrentHealth = maxHealth;
                        OnHealthChanged?.Invoke(CurrentHealth);
                    };
                }
            }

            audioHit.SetCharacterType(characterType);
            oldHealth = maxHealth;
            CurrentHealth = maxHealth;

            OnHealthChanged?.Invoke(CurrentHealth);
        }

        protected override void Die()
        {
            // Disable AI controls
            if (GetComponent<EnemySetup>() != null)
            {
                StateMachine stateMachine = GetComponent<StateMachine>();
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                VisionCollider visionCollider = GetComponentInChildren<VisionCollider>();

                if (stateMachine != null)
                    stateMachine.enabled = false;
                if (agent != null)
                {
                    agent.isStopped = true;
                    agent.enabled = false;
                }
                if (visionCollider != null)
                    visionCollider.enabled = false;
                
            }
            
            base.Die();
        }

        private void OnDestroy()
        {
        }
    }
}
