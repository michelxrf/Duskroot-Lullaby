using UnityEngine;

namespace ProgressionSystem
{
    /// <summary>
    /// Represents a reward object that grants experience points to the player when collected or triggered.
    /// </summary>
    public class Reward : MonoBehaviour
    {
        /// <summary>
        /// Applies the reward by adding experience to the player's current character.
        /// </summary>
        public void ApplyReward()
        {
            int experience = GetComponent<EnemySetup>().GetEnemyData().experienceReward;
            CharacterDataManager.Instance.AddExperience(experience);
        }
}
}
