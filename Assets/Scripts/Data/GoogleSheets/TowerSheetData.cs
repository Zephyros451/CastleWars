using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections.Generic;
using UnityEngine;

public class TowerSheetData : ScriptableObject, ICollectionSet<TowerLevelData>
{
    [SerializeField] protected List<TowerLevelData> towerLevelData;
    public List<TowerLevelData> TowerLevelData => towerLevelData;

    void ICollectionSet<TowerLevelData>.SetCollection(List<TowerLevelData> data)
    {
        towerLevelData = data;
    }
}

[System.Serializable]
public class TowerLevelData
{
    [SerializeField, GoogleSheetParam("quantity_cap")]
    public int quantityCap;
    [SerializeField, GoogleSheetParam("lvl_up_quantity")]
    public int lvlUpQuantity;
    [SerializeField, GoogleSheetParam("generation_rate")]
    public float generationRate;
    [SerializeField, GoogleSheetParam("lvl_up_time")]
    public float lvlUpTime;
    [SerializeField, GoogleSheetParam("attack_tower")]
    public float attackInTower;
    [SerializeField, GoogleSheetParam("hp")]
    public float hp;
}
