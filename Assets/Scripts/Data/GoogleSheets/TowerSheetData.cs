using System.Collections.Generic;
using UnityEngine;
using SIDGIN.Common;
using SIDGIN.GoogleSheets;

public class TowerSheetData : ScriptableObject, ICollectionSet<TowerLevelData>, ICollectionGet<TowerLevelData>
{
    [SerializeField]
    protected List<TowerLevelData> towerLevelData;
    public List<TowerLevelData> TowerLevelData => towerLevelData;

    public List<TowerLevelData> GetCollection()
    {
        return towerLevelData;
    }

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
}
