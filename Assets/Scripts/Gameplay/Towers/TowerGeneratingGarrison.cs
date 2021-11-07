using DG.Tweening;
using System;

public class TowerGeneratingGarrison : TowerGarrison
{
    private float generationRate;

    public TowerGeneratingGarrison(ITower tower, float generationRate) : base(tower)
    {
        this.generationRate = generationRate;
        GarrisonGeneration();
    }

    public void GarrisonGeneration()
    {
        DOTween.Sequence()
            .AppendInterval(generationRate)
            .AppendCallback(() =>
            {
                if (Count < tower.QuantityCap && IsNotUnderAttack && tower.IsNotLevelingUp)
                {
                    units.Push(new UnitData(tower.UnitPrefab.UnitConfigData.GetUnitData(tower.Level)));
                    RaiseCountChanged();
                }
            })
            .SetLoops(-1);
    }
}
