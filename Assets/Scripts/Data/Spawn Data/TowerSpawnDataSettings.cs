using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Spawn Data/Tower Spawn Data Settings")]
public class TowerSpawnDataSettings : ScriptableObject
{
    [SerializeField] private List<TowerSpawnData> swordsmanTowerData;
    [SerializeField] private List<TowerSpawnData> spearmanTowerData;
    [SerializeField] private List<TowerSpawnData> archerTowerData;

    public List<TowerSpawnData> GetData(TowerType towerType)
    {
        switch(towerType)
        {
            case TowerType.Swordsman:
                return swordsmanTowerData;
            case TowerType.Spearman:
                return spearmanTowerData;
            case TowerType.Archer:
                return archerTowerData;
            default:
                Debug.LogError("undefined tower type");
                return null;
        }
    }
}
