using System;
using UnityEngine;

public interface ITower
{
    Transform Transform { get; }
    Tower Tower { get; }
    Allegiance Allegiance { get; }
    float AttackInTower { get; }
    float GenerationRate { get; }
    float HP { get; }
    int LvlUpQuantity { get; }
    float LvlUpTime { get; }
    Model ModelPrefab { get; }
    Navigator Navigator { get; }
    int QuantityCap { get; }
    TowerType TowerType { get; }
    Unit UnitPrefab { get; }
    float GarrisonCount { get; }
    int Level { get; }
    bool IsNotLevelingUp { get; }
    event Action GarrisonCountChanged;
    event Action LevelReseted;
    event Action LevelUpStarted;
    event Action LevelUpEnded;

    void DecreaseGarrisonCount(int amount);
    void ChangeAllegiance(Allegiance allegiance);
    void SetGarrisonCount(float newCount);
}