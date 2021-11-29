using DG.Tweening;

public class TowerGeneratingGarrison : TowerGarrison
{
    private float generationRate;
    private UnitData configUnitData;

    public TowerGeneratingGarrison(ITower tower, float generationRate, UnitData configUnitData) : base(tower, configUnitData)
    {
        this.generationRate = generationRate;
        this.configUnitData = configUnitData;

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
                    units.Push(new UnitData(configUnitData));
                    RaiseCountChanged();
                }
            })
            .SetLoops(-1);
    }
}
