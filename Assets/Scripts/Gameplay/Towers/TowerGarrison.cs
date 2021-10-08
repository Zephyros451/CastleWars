using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TowerGarrison
{
    public event Action CountChanged;

    private ITower tower;

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

    public TowerGarrison(ITower tower, float generationRate)
    {
        this.tower = tower;
        this.generationRate = new WaitForSeconds(generationRate);
        degenerationRate = new WaitForSeconds(0.8f);
    }

    public void DecreaseGarrisonCount(int amount)
    {
        if(amount < 0)
        {
            amount = -amount;
        }

        Count -= amount;
    }

    public IEnumerator GarrisonGeneration()
    {
        while (true)
        {
            yield return generationRate;
            if (Count < tower.QuantityCap && IsNotUnderAttack && tower.IsNotLevelingUp)
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

    public async void AttackedProcessing()
    {
        IsNotUnderAttack = false;
        await Task.Delay(2000);
        IsNotUnderAttack = true;
    }

    public void OnAllyCame()
    {
        Count++;
    }

    public void OnTowerAttacked(IModel model)
    {
        AttackedProcessing();

        if (Mathf.Approximately(Count, 0f))
        {
            tower.ChangeAllegiance(model.Allegiance);
            return;
        }

        int numberOfAttacksBeforeDeath = CalculateNumberOfAttacks(model.HP, tower.AttackInTower);
        Count -= model.Attack * numberOfAttacksBeforeDeath / tower.HP;
    }

    public int CalculateNumberOfAttacks(float HP, float AttackInTower)
    {
        return Mathf.Approximately(HP % AttackInTower, 0f) ? (int)(HP / AttackInTower) : (int)(HP / AttackInTower + 1);
    }
}
