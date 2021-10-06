using System;

public interface ITower
{
    Allegiance Allegiance { get; }
    float AttackInTower { get; }
    float GenerationRate { get; }
    float HP { get; }
    int LvlUpQuantity { get; }
    float LvlUpTime { get; }
    Model ModelPrefab { get; }
    Navigator Navigator { get; }
    int QuantityCap { get; }
    TowerData TowerData { get; }
    TowerType TowerType { get; }
    Unit UnitPrefab { get; }
    float GarrisonCount { get; }
    int Level { get; }
    bool IsNotLevelingUp { get; }

    void DecreaseGarrisonCount(int amount);
    void ChangeAllegiance(Allegiance allegiance);
}