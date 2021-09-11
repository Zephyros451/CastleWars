using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections.Generic;
using UnityEngine;

public class UnitSheetData : ScriptableObject, ICollectionSet<UnitLevelData>
{
    [SerializeField] protected List<UnitLevelData> unitLevelData;
    public List<UnitLevelData> UnitLevelData => unitLevelData;

    void ICollectionSet<UnitLevelData>.SetCollection(List<UnitLevelData> data)
    {
        unitLevelData = data;
    }
}

[System.Serializable]
public class UnitLevelData
{
    [SerializeField, GoogleSheetParam("attack_field")]
    public float attackInField;
    [SerializeField, GoogleSheetParam("hp")]
    public float hp;
    [SerializeField, GoogleSheetParam("speed")]
    public float speed;
}
