using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerGarrison
{
    public virtual event Action CountChanged;

    protected ITower tower;
    protected Stack<UnitData> units = new Stack<UnitData>();

    private float degenerationRate = 0.8f;

    public bool IsNotUnderAttack { get; private set; } = true;

    public int Count
    {
        get
        {
            return units.Count;
        }
    }

    public TowerGarrison(ITower tower)
    {
        this.tower = tower;
        GarrisonDegeneration();
    }

    public void GarrisonDegeneration()
    {
        DOTween.Sequence()
            .AppendInterval(degenerationRate)
            .AppendCallback(() =>
            {
                if ((Count - tower.QuantityCap) > 0f)
                {
                    units.Pop();
                }
            })
            .SetLoops(-1);
    }

    public virtual Stack<UnitData> PopFromGarrison(int amount)
    {
        if(amount < 0)
        {
            amount = -amount;
        }

        var poppedUnits = new Stack<UnitData>();
        for (int i = 0; i < amount || units.Count == 0; i++)
        {
            poppedUnits.Push(units.Pop());
        }
        return poppedUnits;
    }    

    public void AttackedProcessing()
    {
        DOTween.Sequence()
            .AppendCallback(() => { IsNotUnderAttack = false; })
            .AppendInterval(2f)
            .AppendCallback(() => { IsNotUnderAttack = true; });        
    }

    public void OnAllyCame(UnitData ally)
    {
        units.Push(ally);
    }

    public void OnTowerAttacked(IModel model)
    {
        AttackedProcessing();

        if (Count == 0)
        {
            tower.ChangeAllegiance(model.Allegiance);
            return;
        }

        int numberOfAttacksBeforeDeath = CalculateNumberOfAttacks(model.HP, tower.AttackInTower);
        //Count -= model.Attack * numberOfAttacksBeforeDeath / tower.HP;
    }

    public int CalculateNumberOfAttacks(float hp, float AttackInTower)
    {
        return Mathf.Approximately(hp % AttackInTower, 0f) ? (int)(hp / AttackInTower) : (int)((hp / AttackInTower) + 1);
    }

    public int CalculateNumberOfAttacksToKill(float attack, float HPInTower)
    {
        return Mathf.Approximately(HPInTower % attack, 0f) ? (int)(HPInTower / attack) : (int)((HPInTower / attack) + 1);
    }
}
