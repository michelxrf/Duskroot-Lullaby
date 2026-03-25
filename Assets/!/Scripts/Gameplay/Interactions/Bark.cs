using UnityEngine;

[CreateAssetMenu(fileName = "New Bark", menuName = "Scriptable Objects/Bark")]
public class Bark : ScriptableObject
{
    public string text;
    public AudioClip audio;
}
