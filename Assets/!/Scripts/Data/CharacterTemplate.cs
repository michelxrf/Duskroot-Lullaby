using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Template", menuName = "Scriptable Objects/Character Template")]
public class CharacterTemplate : ScriptableObject
{
    // basic info
    public string CharacterId;
    public int Level;
    public int Experience;

    // attributes
    public int health;
    public float armor;
    public int damage;
    public int speed;
    public int attackSpeed;
    public int cure;
    public int hurt;

    // character
    public InventoryItem[] startingItens;
}
