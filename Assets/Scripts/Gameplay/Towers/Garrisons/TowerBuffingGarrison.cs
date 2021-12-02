using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class TowerBuffingGarrison : TowerGarrison
{
    private BuffData buffData;

    private Dictionary<BuffData, UnitData> unitsWaitingForBuff = new Dictionary<BuffData, UnitData>();

    public override int Count
    {
        get => units.Count + unitsWaitingForBuff.Count;
    }

    public override UnitData TopUnit
    {
        get
        {
            if (units.Count == 0) 
            {
                return unitsWaitingForBuff.First().Value;
            }

            return base.TopUnit;
        }
    }

    public TowerBuffingGarrison(ITower tower, BuffData buffData, UnitData configUnitData) : base(tower, configUnitData)
    {
        this.buffData = buffData;

        BuffProcessing();
    }

    public override void GarrisonDegeneration()
    {
        DOTween.Sequence()
            .AppendInterval(degenerationRate)
            .AppendCallback(() =>
            {
                if ((Count - tower.QuantityCap) > 0f)
                {
                    if (unitsWaitingForBuff.Count > 0)
                    {
                        unitsWaitingForBuff.Remove(unitsWaitingForBuff.Last().Key);
                        RaiseCountChanged();
                    }
                    else
                    {
                        units.Pop();
                        RaiseCountChanged();
                    }
                }
            })
            .SetLoops(-1);
    }

    public override Stack<UnitData> PopFromGarrison(int amount)
    {
        if (amount < 0)
        {
            amount = -amount;
        }

        var poppedUnits = new Stack<UnitData>();

        while (amount > 0 && units.Count > 0)
        {
            poppedUnits.Push(units.Pop());
            amount--;
        }

        while(amount > 0 && unitsWaitingForBuff.Count > 0)
        {
            poppedUnits.Push(unitsWaitingForBuff.Last().Value);
            unitsWaitingForBuff.Remove(unitsWaitingForBuff.Last().Key);
            amount--;
        }

        RaiseCountChanged();
        return poppedUnits;
    }

    public override void OnAllyCame(UnitData ally)
    {
        unitsWaitingForBuff.Add(new BuffData(buffData), ally);
        RaiseCountChanged();
    }

    public void BuffProcessing()
    {
        DOTween.Sequence()
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                foreach(var unitBuffPair in unitsWaitingForBuff)
                {
                    unitBuffPair.Key.ApplyBuffTime -= 0.1f;
                }

                for (int i = unitsWaitingForBuff.Count - 1; i >= 0; i--)
                {
                    var key = unitsWaitingForBuff.Keys.ElementAt(i);
                    if (key.ApplyBuffTime > 0f)
                    {
                        continue;
                    }

                    unitsWaitingForBuff[key].ApplyBuff(key);
                    units.Push(unitsWaitingForBuff[key]);
                    unitsWaitingForBuff.Remove(key);
                }
            })
            .SetLoops(-1);
    }
}
