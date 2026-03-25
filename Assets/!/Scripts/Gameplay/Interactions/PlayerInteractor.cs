using UnityEngine;
using Fusion;

public class PlayerInteractor : NetworkBehaviour
{
    Interactions interactionInRange;

    public void EnteredInteractionArea(Interactions interaction)
    {
        interactionInRange = interaction;
    }

    public void LeftInteractionArea()
    {
        interactionInRange = null;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if(interactionInRange == null)
            return;

        // Get input data from the network
        if (GetInput(out NetworkInputData data))
        {
            if (data.Interact)
            {
                Debug.Log("Interacted with " + interactionInRange.name);
                interactionInRange.RPC_ActivateBark();
            }
        }
    }
}
