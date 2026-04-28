using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayersInPlace : NetworkBehaviour
{
    [SerializeField] private UnityEvent onAllPlayersInPlace = new UnityEvent();

    private int playersInTrigger = 0;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<PlayerSetup>() == null)
            return;
        
        playersInTrigger++;
        CheckAllPlayersInPlace();
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponent<PlayerSetup>() == null)
            return;

        playersInTrigger--;
        CheckAllPlayersInPlace();
    }

    private void CheckAllPlayersInPlace()
    {
        if (playersInTrigger == Runner.ActivePlayers.Count())
        {
            onAllPlayersInPlace?.Invoke();
        }
    }
}
