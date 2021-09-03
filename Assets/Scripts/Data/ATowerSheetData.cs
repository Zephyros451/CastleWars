using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Google Sheet Data/A Tower Sheet Data")]
[GoogleSheet("Balance", "Archer Base")]
public class ATowerSheetData : ScriptableObject, ICollectionSet<TowerLevelData>
{
    [SerializeField]
    private List<TowerLevelData> towerLevelData;

    void ICollectionSet<TowerLevelData>.SetCollection(List<TowerLevelData> data)
    {
        towerLevelData = data;
    }
}

[System.Serializable]
public class TowerLevelData
{
    [SerializeField, GoogleSheetParam("quantity_cap")]
    int quantityCap;
    [SerializeField, GoogleSheetParam("lvl_up_quantity")]
    int lvlUpQuantity;
    [SerializeField, GoogleSheetParam("generation_rate")]
    float generationRate;
    [SerializeField, GoogleSheetParam("lvl_up_time")]
    float lvlUpTime;
}
