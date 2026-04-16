using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object that serves as a template for creating character data.
/// Contains all base character attributes, equipment, and progression settings.
/// </summary>
[CreateAssetMenu(fileName = "New Character Template", menuName = "Scriptable Objects/Character Template")]
public class CharacterTemplate : EntityTemplate
{
    public int Experience;
    public Sprite characterPortrait;
    public GameObject characterModel;
}
