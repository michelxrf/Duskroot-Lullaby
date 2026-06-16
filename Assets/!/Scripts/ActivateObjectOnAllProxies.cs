using UnityEngine;
using Fusion;
using Unity.VisualScripting;


/// <summary>
/// This script is used to activate an object on all proxies when the master client activates it.
/// </summary>
public class ActivateObjectOnAllProxies : NetworkBehaviour
{
    [SerializeField] GameObject objectToActivate;

    public void ActivateOnAllProxies()
    {
        RPC_ActivateOnAllProxies();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActivateOnAllProxies()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
