using UnityEngine;

/// <summary>
/// Represents a reward object that grants experience points to the player when collected or triggered.
/// </summary>
public class Reward : MonoBehaviour
{
    [SerializeField] int experience;

    /// <summary>
    /// Applies the reward by adding experience to the player's current character.
    /// </summary>
    public void ApplyReward()
    {
        CharacterDataManager.Instance.AddExperience(experience);
    }
}
