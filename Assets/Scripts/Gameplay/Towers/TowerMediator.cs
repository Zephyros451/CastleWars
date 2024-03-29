﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerMediator : MonoBehaviour, ITower
{
    public event Action GarrisonCountChanged;
    public event Action LevelReseted;
    public event Action LevelUpStarted;
    public event Action LevelUpEnded;

    [Space]
    [SerializeField] private Tower tower;
    [SerializeField] private TowerCollision collision;
    [SerializeField] private TowerTroopSender troopSender;
    [SerializeField] private Navigator navigator;

    public Navigator Navigator => navigator;
    private TowerGarrison Garrison { get; set; }
    private TowerLevel level { get; set; }

    public Transform Transform => transform;
    public Tower Tower => tower;
    public int QuantityCap => tower.TowerSheetData.TowerLevelData[level.Value].quantityCap;
    public int LvlUpQuantity => tower.TowerSheetData.TowerLevelData[level.Value].lvlUpQuantity;
    public float GenerationRate => tower.TowerSheetData.TowerLevelData[level.Value].generationRate;
    public float LvlUpTime => tower.TowerSheetData.TowerLevelData[level.Value].lvlUpTime;
    public float AttackInTower => Garrison.TopUnit.TowerAttack;
    public float HP => Garrison.TopUnit.HP;
    public Unit UnitPrefab => tower.TowerData.unitData.UnitPrefab;
    public Model ModelPrefab => tower.TowerData.unitData.ModelPrefab;
    public Allegiance Allegiance => tower.Allegiance;
    public float GarrisonCount => Garrison.Count;
    public bool IsNotUnderAttack => Garrison.IsNotUnderAttack;
    public int Level => level.Value;
    public bool IsNotLevelingUp => level.IsNotLevelingUp;
    public BuffData BuffData => tower.TowerSheetData.BuffData[level.Value];

    private void Awake()
    {
        level = new TowerLevel(this, 0);
        if (tower.TowerType == TowerType.ArcherGenerating || tower.TowerType == TowerType.SwordsmanGenerating)
        {
            Garrison = new TowerGeneratingGarrison(this, GenerationRate, tower.TowerData.unitData.GetUnitData(level.Value));
        }
        else
        {
            Garrison = new TowerBuffingGarrison(this, this.BuffData, tower.TowerData.unitData.GetUnitData(level.Value));
        }
    }

    private void Start()
    {
        collision.TowerAttacked += OnTowerAttacked;
        collision.AllyCame += OnTowerGotBackUp;
        Garrison.CountChanged += OnGarrisonCountChanged;
        level.LevelUpStarted += OnLevelUpStarted;
        level.LevelUpEnded += OnLevelUpEnded;
        level.LevelReseted += OnLevelReseted;
    }

    private void OnDestroy()
    {
        collision.TowerAttacked -= OnTowerAttacked;
        collision.AllyCame -= OnTowerGotBackUp;
        Garrison.CountChanged -= OnGarrisonCountChanged;
        level.LevelUpStarted -= OnLevelUpStarted;
        level.LevelUpEnded -= OnLevelUpEnded;
        level.LevelReseted -= OnLevelReseted;
    }

    private void OnTowerAttacked(Model model)
    {
        Garrison.OnTowerAttacked(model);
    }

    private void OnTowerGotBackUp(UnitData unitData)
    {
        Garrison.OnAllyCame(unitData);
    }

    private void OnGarrisonCountChanged()
    {
        GarrisonCountChanged?.Invoke();
    }

    private void OnLevelReseted()
    {
        LevelReseted?.Invoke();
    }

    private void OnLevelUpStarted()
    {
        LevelUpStarted?.Invoke();
    }

    private void OnLevelUpEnded()
    {
        LevelUpEnded?.Invoke();
    }

    public void SendTroopTo(ITower anotherTower)
    {
        troopSender.SendTroopTo(anotherTower);
    }

    void ITower.ChangeAllegiance(Allegiance newAllegiance)
    {
        level.Reset();
        level.IsNotLevelingUp = true;
        tower.ChangeAllegiance(newAllegiance);
    }

    Stack<UnitData> ITower.PopFromGarrison(int amount)
    {
        return Garrison.PopFromGarrison(amount);
    }

    void ITower.DecreaseGarrisonCount(float newCount)
    {
        if (newCount < 0f)
        {
            Debug.LogError("garrison count cannot be less than zero");
            return;
        }

        if (newCount > GarrisonCount)
        {
            Debug.LogError("new garrison count cannot be greater than it was previously");
            return;
        }

        while (newCount < GarrisonCount)
        {
            Garrison.PopFromGarrison(1);
        }
    }

    public void LevelUp()
    {
        level.LevelUp();
    }

    public void Reset()
    {
        tower = GetComponent<Tower>();
        navigator = GetComponent<Navigator>();
        collision = GetComponentInChildren<TowerCollision>();
        troopSender = GetComponent<TowerTroopSender>();
    }

    public void ReceiveDamage(float damage)
    {
        if(this.GarrisonCount == 0)
        {
            return;
        }

        this.Garrison.TopUnit.DecreaseHP(damage);

        if(this.Garrison.TopUnit.HP < 0f)
        {
            this.Garrison.PopFromGarrison(1);
        }
    }

    public void Destroy()
    {
        Navigator.Destroy();
    }
}
