using System;
using System.Threading.Tasks;
using UnityEngine;

public class TowerLevel
{
    public event Action LevelUpStarted;
    public event Action LevelUpEnded;
    public event Action LevelReseted;

    private Tower tower;
    public bool IsNotLevelingUp = true;
    private WaitForSeconds LevelUpCooldown = new WaitForSeconds(5f);
    private Coroutine levelUpCoroutine;

    public int Value { get; private set; } = 0;

    public TowerLevel(Tower tower, int startingLevel)
    {
        this.tower = tower;
        Value = startingLevel;
    }

    public void Reset()
    {
        if (Value > 0)
        {
            Value = 0;
            LevelReseted?.Invoke();
        }
    }

    public void LevelUp()
    {
        if (tower.Garrison.Count < tower.LvlUpQuantity)
            return;

        tower.Garrison.Count -= tower.LvlUpQuantity;
        Value++;

        LevelUpProcessing();
    }

    private async void LevelUpProcessing()
    {
        IsNotLevelingUp = false;
        LevelUpStarted?.Invoke();
        await Task.Delay(5000);
        IsNotLevelingUp = true;
        LevelUpEnded?.Invoke();
    }
}
