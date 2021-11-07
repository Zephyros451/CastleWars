using SIDGIN.Common;
using SIDGIN.GoogleSheets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Google Sheet Data/Buff Sheet Data")]
[GoogleSheet("Balance", "Buffs")]
public class BuffSheetData : ScriptableObject, ICollectionSet<BuffData>
{
    [SerializeField] private List<BuffData> buffLevelData;
    public List<BuffData> BuffLevelData => buffLevelData;

    void ICollectionSet<BuffData>.SetCollection(List<BuffData> data)
    {
        buffLevelData = data;
    }
}

[System.Serializable]
public class BuffData
{
    [SerializeField, GoogleSheetParam("hp")]
    public float hp;
    [SerializeField, GoogleSheetParam("attack")]
    public float attack;
    [SerializeField, GoogleSheetParam("apply_time")]
    public float applyTime;

    public BuffData(BuffData buffData)
    {
        this.hp = buffData.hp;
        this.attack = buffData.attack;
        this.applyTime = buffData.applyTime;
    }
}
