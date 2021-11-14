using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections.Generic;
using UnityEngine;

public class TowerSheetData : ScriptableObject, ICollectionSet<TowerData>
{
    [SerializeField] protected List<TowerData> towerData;
    [SerializeField] protected List<BuffData> buffData;
    [SerializeField] protected UnitSheetData unitDatas;

    public List<TowerData> TowerLevelData => towerData;
    public List<BuffData> BuffData => buffData;
    public UnitSheetData UnitDatas => unitDatas;

    void ICollectionSet<TowerData>.SetCollection(List<TowerData> data)
    {
        towerData = data;
    }
}

[System.Serializable]
public class TowerData
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

[System.Serializable]
public class BuffData
{
    [SerializeField, GoogleSheetParam("hp")]
    public float HP;
    [SerializeField, GoogleSheetParam("attack")]
    public float attack;
    [SerializeField, GoogleSheetParam("apply_time")]
    public float applyTime;

    public BuffData(BuffData buffData)
    {
        this.HP = buffData.HP;
        this.attack = buffData.attack;
        this.applyTime = buffData.applyTime;
    }
}
