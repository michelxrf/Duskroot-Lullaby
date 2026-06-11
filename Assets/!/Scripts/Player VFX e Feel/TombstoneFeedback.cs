using UnityEngine;
using MoreMountains.Feedbacks;


public class TombstoneFeedback : MonoBehaviour
{
    [SerializeField] private MMF_Player spawnFeedback;
    [SerializeField] private MMF_Player destroyFeedback;

    public float DestroyFeedbackDuration => destroyFeedback.TotalDuration;
    public void PlaySpawn()
    {
        spawnFeedback?.PlayFeedbacks();
    }
    public void DestroyTombstone()
    {
        destroyFeedback?.PlayFeedbacks();
    }
}
