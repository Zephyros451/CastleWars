using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Spawn Data/Tower Spawn Data Settings")]
public class TowerSpawnDataSettings : ScriptableObject
{
    [SerializeField] private List<TowerSpawnData> swordsmanTowerData;
    [SerializeField] private List<TowerSpawnData> spearmanTowerData;
    [SerializeField] private List<TowerSpawnData> archerTowerData;

    public List<TowerSpawnData> GetData(UnitType towerType)
    {
        switch(towerType)
        {
            case UnitType.Swordsman:
                return swordsmanTowerData;
            case UnitType.Spearman:
                return spearmanTowerData;
            case UnitType.Archer:
                return archerTowerData;
            default:
                Debug.LogError("undefined tower type");
                return null;
        }
    }
}
