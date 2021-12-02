using System;
using System.Threading.Tasks;

public class TowerLevel
{
    public event Action LevelUpStarted;
    public event Action LevelUpEnded;
    public event Action LevelReseted;

    private ITower tower;
    public bool IsNotLevelingUp = true;

    public int Value { get; private set; } = 0;

    public TowerLevel(ITower tower, int startingLevel)
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
        if (tower.GarrisonCount < tower.LvlUpQuantity)
            return;

        tower.PopFromGarrison(tower.LvlUpQuantity);
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
