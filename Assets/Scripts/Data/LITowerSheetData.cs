using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Google Sheet Data/LI Tower Sheet Data")]
[GoogleSheet("Balance", "Swordsman Base")]
public class LITowerSheetData : ScriptableObject, ICollectionSet<TowerLevelData>
{
    [SerializeField]
    private List<TowerLevelData> towerLevelData;

    void ICollectionSet<TowerLevelData>.SetCollection(List<TowerLevelData> data)
    {
        towerLevelData = data;
    }
}
