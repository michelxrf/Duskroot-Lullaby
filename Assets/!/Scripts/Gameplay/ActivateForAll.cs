using Fusion;
using UnityEngine;

public class ActivateForAll : NetworkBehaviour
{
    [SerializeField] GameObject targetObject;

    public override void Spawned()
    {
        // Activate the GameObject for all players when it spawns
        RPC_Activate(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Activate(bool state)
    {
        targetObject.SetActive(state);
    }

}
