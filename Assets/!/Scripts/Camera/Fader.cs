using UnityEngine;

/// <summary>
/// Used for props to prevent them from blocking player's view
/// </summary>
public class Fader : MonoBehaviour
{
    public void Fade()
    {
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
    }

    public void UnFade()
    {
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
    }

    // TODO: Make the fading smooth with a coroutine
}
