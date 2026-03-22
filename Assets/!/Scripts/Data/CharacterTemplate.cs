using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object that serves as a template for creating character data.
/// Contains all base character attributes, equipment, and progression settings.
/// </summary>
[CreateAssetMenu(fileName = "New Character Template", menuName = "Scriptable Objects/Character Template")]
public class CharacterTemplate : ScriptableObject
{
    // basic info
    /// <summary>Unique identifier for this character</summary>
    public string CharacterId;

    /// <summary>Starting level for characters created from this template</summary>
    public int Level;

    /// <summary>Starting experience points</summary>
    public int Experience;

    /// <summary>Experience required to reach the next level</summary>
    public int ExperienceToNextLevel;

    // attributes
    /// <summary>Maximum health points</summary>
    public int health;

    /// <summary>Damage reduction multiplier</summary>
    public float armor;

    /// <summary>Base damage dealt by attacks</summary>
    public int damage;

    /// <summary>Movement speed</summary>
    public int speed;

    /// <summary>Attack speed (attacks per second)</summary>
    public int attackSpeed;

    /// <summary>Healing or cure ability value</summary>
    public int cure;

    // inventory
    /// <summary>The starting weapon for this character</summary>
    public WeaponData weapon;

    /// <summary>Portrait image to display for this character in the UI</summary>
    public Sprite characterPortrait;
}
