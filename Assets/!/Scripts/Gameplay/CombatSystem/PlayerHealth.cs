using CombatSystem;
using UnityEngine;

namespace CombatSystem
{
    public class PlayerHealth : Health
    {
        public override void Spawned()
        {
            base.Spawned();

            // player Init
            if (GetComponent<PlayerAttack>() != null)
            {
                maxHealth = CharacterDataManager.Instance.GetCurrentPlayerCharacter().health;
                characterType = CharacterDataManager.Instance.GetCurrentPlayerCharacter().characterId;

                CharacterDataManager.Instance.OnLevelUp += () => { maxHealth = CharacterDataManager.Instance.GetCurrentPlayerCharacter().health; };
            }

            oldHealth = maxHealth;
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        protected override void Die()
        {
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<CharacterLook>().enabled = false;

            base.Die();
        }

        public void RPC_Revive()
        {
            GetComponent<PlayerAttack>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<CharacterLook>().enabled = true;
            
            CurrentHealth = maxHealth / 4;
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
    }

}