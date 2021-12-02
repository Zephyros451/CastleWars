using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Spawn Data/Tower Spawn Data Settings")]
public class TowerSpawnDataSettings : ScriptableObject
{
    [SerializeField] private List<TowerSpawnData> swordsmanTowerData;
    [SerializeField] private List<TowerSpawnData> archerTowerData;
    [SerializeField] private List<TowerSpawnData> armoryTowerData;
    [SerializeField] private List<TowerSpawnData> magicTowerData;

    public List<TowerSpawnData> GetData(TowerType towerType)
    {
        switch(towerType)
        {
            case TowerType.SwordsmanGenerating:
                return swordsmanTowerData;
            case TowerType.ArcherGenerating:
                return archerTowerData;
            case TowerType.HPBuff:
                return armoryTowerData;
            case TowerType.AttackBuff:
                return magicTowerData;
            default:
                Debug.LogError("undefined tower type");
                return null;
        }
    }
}
