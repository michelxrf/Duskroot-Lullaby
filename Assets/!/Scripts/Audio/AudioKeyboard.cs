using UnityEngine;
using FMODUnity;

public class AudioKeyboard : MonoBehaviour
{
    [SerializeField] private EventReference keySound;

    public void PressKey()
    {
        RuntimeManager.PlayOneShot(keySound);
    }
}