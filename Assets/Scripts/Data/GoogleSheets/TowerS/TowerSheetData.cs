using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections.Generic;
using UnityEngine;

public class TowerSheetData : ScriptableObject, ICollectionSet<SheetData>
{
    [SerializeField] protected List<SheetData> towerData;
    [SerializeField] protected UnitSheetData unitDatas;

    protected List<BuffData> buffData = new List<BuffData>();

    public List<SheetData> TowerLevelData => towerData;
    public UnitSheetData UnitDatas => unitDatas;
    public List<BuffData> BuffData
    {
        get
        {
            if(buffData.Count == 0)
            {
                foreach (var item in towerData)
                {
                    buffData.Add(new BuffData(item.hpBuff, item.attackBuff, item.applyBuffTime));
                }
            }
            return buffData;
        }
    }

    void ICollectionSet<SheetData>.SetCollection(List<SheetData> data)
    {
        towerData = data;
    }
}

[System.Serializable]
public class SheetData
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
    [SerializeField, GoogleSheetParam("hp_buff")]
    public float hpBuff;
    [SerializeField, GoogleSheetParam("attack_buff")]
    public float attackBuff;
    [SerializeField, GoogleSheetParam("apply_time")]
    public float applyBuffTime;
}

public class BuffData
{
    public float HP;
    public float Attack;
    public float ApplyBuffTime;

    public BuffData(float hp, float attack, float applyBuffTime)
    {
        HP = hp;
        Attack = attack;
        ApplyBuffTime = applyBuffTime;
    }

    public BuffData(BuffData buffData)
    {
        HP = buffData.HP;
        Attack = buffData.Attack;
        ApplyBuffTime = buffData.ApplyBuffTime;
    }
}