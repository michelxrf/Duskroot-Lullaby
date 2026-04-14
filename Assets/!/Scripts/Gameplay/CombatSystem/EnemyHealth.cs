using CombatSystem;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

namespace CombatSystem
{
    public class EnemyHealth : Health
    {

        public override void Spawned()
        {
            base.Spawned();

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
                        oldHealth = maxHealth;
                        CurrentHealth = maxHealth;
                    };
                }
            }

            oldHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        protected override void Die()
        {
            // Disable AI controls
            if (GetComponent<EnemySetup>() != null)
            {
                gameObject.DisableAllComponents<StateMachine>();
                NavMeshAgent agent = GetComponent<NavMeshAgent>();

                if (agent != null)
                    agent.isStopped = true;
            }
            
            base.Die();
        }
    }
}
