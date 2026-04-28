using UnityEngine;

public class EnableAllPlayers : MonoBehaviour
{
    public void EnableAll(bool newState)
    {
        PlayerSetup[] allPlayers = FindObjectsByType<PlayerSetup>(FindObjectsSortMode.None);
        foreach (PlayerSetup player in allPlayers)
        {
            player.EnablePlayerControls(newState);
        }
    }
}
