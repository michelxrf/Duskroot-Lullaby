using UnityEngine;

public class MusMenuStart : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Toca musica no start");
        MusicManager.instance.StartMusic();

    }

}
