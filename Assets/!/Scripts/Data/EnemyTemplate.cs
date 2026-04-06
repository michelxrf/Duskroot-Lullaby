using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Template", menuName = "Scriptable Objects/Enemy Template")]
public class EnemyTemplate : EntityTemplate
{
    public int experienceReward;
    public int experienceRewardUpgradePerLevel;
}
