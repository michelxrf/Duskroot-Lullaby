using UnityEngine;

public class MusAreaSelector : MonoBehaviour
{
    [SerializeField] private int musicParam = 1;
    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;

        //if (other.CompareTag("Player") && otherPlayer.IsLocalPlayer() && !isPlayerInside)
        if (otherPlayer.IsLocalPlayer() && !isPlayerInside)
            {
            isPlayerInside = true;
            MusLevel1.instance.SetLevelMusicParameter(musicParam);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;
        if (otherPlayer.IsLocalPlayer() && isPlayerInside)
        //if (other.CompareTag("Player") && otherPlayer.IsLocalPlayer() && isPlayerInside)
        {
            isPlayerInside = false;
            MusLevel1.instance.SetLevelMusicParameter(1);

        }
    }
}