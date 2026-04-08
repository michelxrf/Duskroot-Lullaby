using CombatSystem;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/// <summary>
/// Represents a single player character card displayed in the UI.
/// Handles the visual display of health, experience, character portrait, and status indicators.
/// </summary>
public class PlayerCard : MonoBehaviour
{
    [SerializeField] Image healthBarFill;
    [SerializeField] TMP_Text healthBarLabel;
    [SerializeField] Image expBarFill;
    [SerializeField] TMP_Text expBarLabel;
    [SerializeField] Image characterPortrait;
    [SerializeField] Image deadIcon;
    [SerializeField] Image highlight;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text level;

    string characterId;
    Health health;

    /// <summary>
    /// Initializes the player card with character data and health component.
    /// Sets up event listeners for health and experience changes.
    /// </summary>
    /// <param name="character">The character data to display</param>
    /// <param name="healthComp">The health component to listen to for health changes</param>
    public void Initialize(CharacterData character, Health healthComp)
    {
        deadIcon.gameObject.SetActive(false);
        characterPortrait.sprite = CharacterDataManager.Instance.GetCharacterPortrait(character.characterId);
        health = healthComp;

        characterId = character.characterId;

        highlight.gameObject.SetActive(character.characterId == CharacterDataManager.Instance.localPlayerCharacterId);

        healthComp.OnHealthChanged += (int value) => UpdateHealth(value);
        CharacterDataManager.Instance.OnExpChanged += UpdateExperience;
        CharacterDataManager.Instance.OnLevelUp += () => UpdateHealth(health.CurrentHealth);
        CharacterDataManager.Instance.OnLevelUp += () => UpdateLevel(CharacterDataManager.Instance.GetCharacter(characterId).level);

        characterName.text = character.characterId;
        UpdateLevel(character.level);
        UpdateHealth(healthComp.CurrentHealth);
        UpdateExperience(character.experience);
    }

    /// <summary>
    /// Updates the health bar display with the current health value.
    /// </summary>
    /// <param name="currentHealth">The current health value</param>
    public void UpdateHealth(int currentHealth)
    {
        CharacterData characterData = CharacterDataManager.Instance.GetCharacter(characterId);

        healthBarFill.fillAmount = (float)currentHealth / characterData.health;
        healthBarLabel.text = $"{currentHealth}/{characterData.health}";
    }

    /// <summary>
    /// Updates the experience bar display with the current experience value.
    /// </summary>
    /// <param name="currentExp">The current experience value</param>
    public void UpdateExperience(int currentExp)
    {
        CharacterData characterData = CharacterDataManager.Instance.GetCharacter(characterId);

        expBarFill.fillAmount = (float)currentExp / characterData.experienceToNextLevel;
        expBarLabel.text = $"{currentExp}/{characterData.experienceToNextLevel}";
    }

    public void UpdateLevel(int newLevel)
    {
        level.text = $"Lvl: {newLevel}";
    }

    /// <summary>
    /// Unsubscribes from events when the card is destroyed to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        CharacterDataManager.Instance.OnExpChanged -= UpdateExperience;
    }
}
