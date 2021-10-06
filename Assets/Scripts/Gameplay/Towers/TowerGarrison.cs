using System;
using System.Collections;
using UnityEngine;

public class TowerGarrison
{
    public event Action CountChanged;

    private Tower tower;

    private WaitForSeconds generationRate;
    private WaitForSeconds degenerationRate;
    private WaitForSeconds attackedCooldown = new WaitForSeconds(2f);

    public bool IsNotUnderAttack { get; private set; } = true;

    private float count = 10;
    public float Count
    {
        get
        {
            return count;
        }
        set
        {
            count = Mathf.Clamp(value, 0f, float.MaxValue);
            CountChanged?.Invoke();
        }
    }

    public TowerGarrison(Tower tower, TowerSheetData data)
    {
        this.tower = tower;
        generationRate = new WaitForSeconds(data.TowerLevelData[tower.Level.Value].generationRate);
        degenerationRate = new WaitForSeconds(0.8f);
        tower.Collision.TowerAttacked += OnTowerAttacked;
        tower.Collision.AllyCame += OnAllyCame;
    }

    public IEnumerator GarrisonGeneration()
    {
        while (true)
        {
            yield return generationRate;
            if (Count < tower.QuantityCap && IsNotUnderAttack && tower.Level.IsNotLevelingUp)
            {
                Count++;
            }
        }
    }

    public IEnumerator GarrisonDegeneration()
    {
        while (true)
        {
            yield return degenerationRate;
            if ((Count - tower.QuantityCap) >= 1f)
            {
                Count--;
            }
        }
    }

    public IEnumerator AttackedProcessing()
    {
        IsNotUnderAttack = false;
        yield return attackedCooldown;
        IsNotUnderAttack = true;
    }

    private void OnAllyCame()
    {
        tower.Garrison.Count++;
    }

    private void OnTowerAttacked(Model model)
    {
        if (Mathf.Approximately(Count, 0f))
        {
            tower.ChangeAllegiance(model.Allegiance);
            return;
        }

        int numberOfAttacksBeforeDeath = Mathf.Approximately(model.HP % tower.AttackInTower, 0f) ?
                                         (int)(model.HP / tower.AttackInTower) : (int)(model.HP / tower.AttackInTower + 1);
        Count -= model.Attack * numberOfAttacksBeforeDeath / tower.HP;
    }
}
