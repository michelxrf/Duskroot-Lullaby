using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bark", menuName = "Scriptable Objects/Bark")]
public class Bark : ScriptableObject
{
    public string text;
    [Tooltip("Valor do parâmetro BarkNumber no FMOD")]
    public int barkNumber; // 1, 2, 3...
}