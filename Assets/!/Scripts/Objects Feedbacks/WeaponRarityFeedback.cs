using MoreMountains.Feedbacks;
using UnityEngine;

public class WeaponRarityFeedback : MonoBehaviour
{
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player commonFeedback;
    [SerializeField] private MMF_Player uncommonFeedback;
    [SerializeField] private MMF_Player rareFeedback;
    [SerializeField] private MMF_Player legendaryFeedback;

    public void PlayRarityFeedback(int rarity)
    {
        StopAllFeedbacks();

        switch (rarity)
        {
            case 0:
                commonFeedback?.PlayFeedbacks();
                break;

            case 1:
                uncommonFeedback?.PlayFeedbacks();
                break;

            case 2:
                rareFeedback?.PlayFeedbacks();
                break;

            case 3:
                legendaryFeedback?.PlayFeedbacks();
                break;
        }
    }

    private void StopAllFeedbacks()
    {
        commonFeedback?.StopFeedbacks();
        uncommonFeedback?.StopFeedbacks();
        rareFeedback?.StopFeedbacks();
        legendaryFeedback?.StopFeedbacks();
    }
}