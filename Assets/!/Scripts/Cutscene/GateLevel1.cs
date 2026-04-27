using UnityEngine;

public class GateLevel1 : MonoBehaviour
{
    [SerializeField] private GameObject timeline;

    private void Awake()
    {
        if(timeline != null) timeline.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;

        //if (other.CompareTag("Player") && otherPlayer.IsLocalPlayer() && !isPlayerInside)
        if (otherPlayer.IsLocalPlayer())
        {
            MusLevel1.instance.StopMusic();
            if (timeline != null)  timeline.SetActive(true);
            
        }
    }
}
