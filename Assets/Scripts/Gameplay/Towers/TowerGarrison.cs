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

    public TowerGarrison() { }

    public TowerGarrison(ITower tower, TowerSheetData data)
    {
        this.tower = tower;
        generationRate = new WaitForSeconds(data.TowerLevelData[tower.Level].generationRate);
        degenerationRate = new WaitForSeconds(0.8f);
    }

    public void DecreaseGarrisonCount(int amount)
    {
        Count -= amount;
    }

    public void SetGarrisonCount(float newCount)
    {
        Count = newCount;
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

    public void OnTowerAttacked(Model model)
    {
        AttackedProcessing();

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
