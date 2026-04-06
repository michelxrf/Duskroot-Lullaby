using UnityEngine;
using Fusion;
using ProgressionSystem;
using System;

public class EnemySetup : NetworkBehaviour
{
    [SerializeField] EnemyTemplate enemyTemplate;
    [SerializeField] int level;
    [Networked] public string characterId { get => default; set { } }

    EnemyData enemyData;
    bool isInitialized = false;
    public Action OnInit;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            if(level < 1)
                level = 1;

            enemyData = ExperienceCalculator.LevelUpEnemy(enemyTemplate, level);
            enemyData.CharacterId = enemyTemplate.CharacterId;

            isInitialized = true;
            OnInit?.Invoke();
        }
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public EnemyData GetEnemyData()
    {
        return enemyData;
    }

    public string GetCharacterId()
    {
        return enemyData.CharacterId;
    }
}
