using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

namespace CombatSystem
{
    /// <summary>
    /// Handles player input for initiating attacks
    /// </summary>
    public class PlayerAttack : NetworkBehaviour
    {
        bool lastAttack = false; // to make it so attack only triggers on button down

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            if (GetInput(out NetworkInputData data))
            {
                if (data.Attack && !lastAttack)
                {
                    GetComponent<Attack>().InitiateAttack();
                }

                lastAttack = data.Attack;
            }
        }
    }
}

