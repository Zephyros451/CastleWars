using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuffingGarrison : TowerGarrison
{
    private float buffApplyTime;
    private BuffSheetData buffData;

    private Dictionary<UnitData, BuffData> unitsWaitingForBuff = new Dictionary<UnitData, BuffData>();

    public TowerBuffingGarrison(ITower tower, float buffApplyTime, BuffSheetData buffData) : base(tower)
    {
        this.buffApplyTime = buffApplyTime;
        this.buffData = buffData;
    }

    public override void OnAllyCame(UnitData ally)
    {
        unitsWaitingForBuff.Add(ally, new BuffData(buffData.BuffLevelData[tower.Level]));
    }

    public void BuffProcessing()
    {
        DOTween.Sequence()
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                foreach(var unitBuffPair in unitsWaitingForBuff)
                {
                    unitBuffPair.Value.applyTime -= 0.1f;
                    if (unitBuffPair.Value.applyTime < 0f)
                    {
                        ApplyBuff(unitBuffPair);
                    }
                }
            })
            .SetLoops(-1);
    }

    private void ApplyBuff(KeyValuePair<UnitData, BuffData> unitBuffPair)
    {
        unitBuffPair.Key.ApplyBuff(unitBuffPair.Value);

    }
}
