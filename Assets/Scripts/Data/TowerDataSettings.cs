using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/TowerDataSettings")]
public class TowerDataSettings : ScriptableObject
{
    [SerializeField] private List<TowerData> swordsmanTowerData;
    [SerializeField] private List<TowerData> spearmanTowerData;
    [SerializeField] private List<TowerData> archerTowerData;

    public List<TowerData> GetData(TowerType towerType)
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
