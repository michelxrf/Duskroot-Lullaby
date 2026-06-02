using Fusion;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component that allows a GameObject to be picked up as a key.
/// Holds a list of strings representing the keys.
/// </summary>
public class PickableKey : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] GameObject interactionTooltip;
    [SerializeField] List<string> keys = new List<string>();

    int playersInRange = 0;

    /// <summary>
    /// Initializes the key list.
    /// </summary>
    /// <param name="newKeys">The keys to hold.</param>
    public void Initialize(IEnumerable<string> newKeys)
    {
        keys = new List<string>(newKeys);
    }

    /// <summary>
    /// Returns the list of keys held by this object.
    /// </summary>
    public List<string> GetKeys()
    {
        return keys;
    }

    public override void Spawned()
    {
        if (interactionTooltip != null)
            interactionTooltip.SetActive(false);

        if (keys == null || keys.Count == 0)
        {
            Debug.LogError($"PickableKey on {gameObject.name} spawned with an empty key list!", gameObject);
        }
    }


    private void Update()
    {
        if (interactionTooltip != null)
            interactionTooltip.SetActive(playersInRange > 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange++;
        other.GetComponent<PlayerInteractor>()?.EnteredPickableKeyArea(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerSetup>() == null)
            return;

        playersInRange--;
        other.GetComponent<PlayerInteractor>()?.LeftPickableKeyArea(this);
    }

    /// <summary>
    /// Despawns the key object.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Despawn()
    {
        Runner.Despawn(Object);
    }
}
