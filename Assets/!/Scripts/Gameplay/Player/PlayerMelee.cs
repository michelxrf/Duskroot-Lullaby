using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class PlayerMelee : NetworkBehaviour
{
    bool lastAttack = false; // to make it so attack only triggers on button down

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if(GetInput(out NetworkInputData data))
        {
            if (data.Attack && !lastAttack)
            {
                GetComponent<MeleeAttack>().InitiateAttack();
            }

            lastAttack = data.Attack;
        }
    }
}
