using UnityEngine;


/// <summary>
/// Used by the camera to trigger fading in objects that obstruct the player's view
/// </summary>
public class FadeVisualObstacles : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Fader>()?.Fade();
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Fader>()?.UnFade();
    }
}
