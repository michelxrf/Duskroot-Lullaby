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
    /// <summary>
    /// Creates and initializes a new player card for the given character.
    /// </summary>
    public void Initialize(CharacterData character, PlayerHealth healthComp)
    {
        GameObject newCard = Instantiate(playerCardPrefab);

        // Importante para UI
        newCard.transform.SetParent(transform, false);

        RectTransform rect = newCard.GetComponent<RectTransform>();

        // Size
        rect.sizeDelta = new Vector2(470f, 265f);

        // Position (UI position)
        rect.anchoredPosition = new Vector2(280f, 180f);

        newCard.GetComponent<PlayerCard>()
            .Initialize(character, healthComp);
    }
}
