using UnityEngine;

public class MusMenuStart : MonoBehaviour
{
    private void Start()
    {
        MusicManager.instance.StartMusic();
    }
}
