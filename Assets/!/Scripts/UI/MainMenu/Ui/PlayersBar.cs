using CombatSystem;
using UnityEngine;

/// <summary>
/// Manages the display of player character cards in the UI.
/// Spawns and initializes PlayerCard instances for each character.
/// </summary>
public class PlayersBar : MonoBehaviour
{
    [SerializeField] GameObject playerCardPrefab;

    /// <summary>
    /// Creates and initializes a new player card for the given character.
    /// </summary>
    /// <param name="character">The character data to create a card for</param>
    /// <param name="healthComp">The health component of the character</param>
    public void Initialize(CharacterData character, Health healthComp)
    {
        GameObject newCard = Instantiate(playerCardPrefab);
        newCard.transform.SetParent(transform);
        newCard.GetComponent<PlayerCard>().Initialize(character, healthComp);
    }
}
