using System;

public interface ITower
{
    Allegiance Allegiance { get; }
    float AttackInTower { get; }
    TowerCollision Collision { get; }
    TowerGarrison Garrison { get; }
    float GenerationRate { get; }
    float HP { get; }
    TowerLevel Level { get; }
    int LvlUpQuantity { get; }
    float LvlUpTime { get; }
    Model ModelPrefab { get; }
    Navigator Navigator { get; }
    int QuantityCap { get; }
    TowerData TowerData { get; }
    TowerType TowerType { get; }
    TowerTroopSender TroopSender { get; }
    Unit UnitPrefab { get; }


    void ChangeAllegiance(Allegiance newAllegiance);
}