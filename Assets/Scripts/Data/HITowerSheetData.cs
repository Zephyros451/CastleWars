﻿using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Google Sheet Data/HI Tower Sheet Data")]
[GoogleSheet("Balance", "Spearman Base")]
public class HITowerSheetData : ScriptableObject, ICollectionSet<TowerLevelData>
{
    [SerializeField]
    private List<TowerLevelData> towerLevelData;

    void ICollectionSet<TowerLevelData>.SetCollection(List<TowerLevelData> data)
    {
        towerLevelData = data;
    }
}