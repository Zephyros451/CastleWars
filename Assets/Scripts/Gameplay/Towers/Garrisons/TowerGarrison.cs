using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerGarrison
{
    public event Action CountChanged;

    protected ITower tower;
    protected Stack<UnitData> units = new Stack<UnitData>();

    protected readonly float degenerationRate = 0.8f;

    public bool IsNotUnderAttack { get; private set; } = true;

    public virtual UnitData TopUnit => units.Peek();

    public virtual int Count
    {
        get => units.Count;
    }

    public TowerGarrison(ITower tower)
    {
        this.tower = tower;
        GarrisonDegeneration();
    }

    protected void RaiseCountChanged()
    {
        CountChanged?.Invoke();
    }

    public virtual void GarrisonDegeneration()
    {
        DOTween.Sequence()
            .AppendInterval(degenerationRate)
            .AppendCallback(() =>
            {
                if ((Count - tower.QuantityCap) > 0f)
                {
                    units.Pop();
                    RaiseCountChanged();
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
        for (int i = 0; i < amount && units.Count > 0; i++)
        {
            poppedUnits.Push(units.Pop());
        }
        RaiseCountChanged();
        return poppedUnits;
    }    

    public void AttackedProcessing()
    {
        DOTween.Sequence()
            .AppendCallback(() => { IsNotUnderAttack = false; })
            .AppendInterval(2f)
            .AppendCallback(() => { IsNotUnderAttack = true; });        
    }

    public virtual void OnAllyCame(UnitData ally)
    {
        units.Push(ally);
        RaiseCountChanged();
    }

    public void OnTowerAttacked(IModel model)
    {
        if (Count == 0)
        {
            tower.ChangeAllegiance(model.Allegiance);
            return;
        }

        if (model.Attack == 0f || tower.AttackInTower == 0)
        {
            Debug.LogError($"attack is zero");
            return;
        }

        AttackedProcessing();

        var defenderHP = tower.HP;
        var attackerHP = model.HP;

        while (attackerHP > 0f) 
        {
            defenderHP -= model.Attack;
            tower.ReceiveDamage(model.Attack);
            if (Count == 0)
            {
                tower.ChangeAllegiance(model.Allegiance);
                return;
            }
            if (defenderHP < 0f)
            {
                tower.DecreaseGarrisonCount(tower.GarrisonCount - 1);
            }

            if (Count == 0)
            {
                tower.ChangeAllegiance(model.Allegiance);
                return;
            }

            attackerHP -= tower.AttackInTower;
        }
    }
}
