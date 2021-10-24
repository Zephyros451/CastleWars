using System;
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
    public float AttackInTower => tower.TowerSheetData.TowerLevelData[level.Value].attackInTower;
    public float HP => tower.TowerSheetData.TowerLevelData[level.Value].hp;
    public Unit UnitPrefab => tower.TowerData.unitData.UnitPrefab;
    public Model ModelPrefab => tower.TowerData.unitData.ModelPrefab;
    public Allegiance Allegiance => tower.Allegiance;
    public UnitType TowerType => tower.TowerType;
    public float GarrisonCount => Garrison.Count;
    public bool IsNotUnderAttack => Garrison.IsNotUnderAttack;
    public int Level => level.Value;
    public bool IsNotLevelingUp => level.IsNotLevelingUp;

    private void Awake()
    {
        level = new TowerLevel(this, 0);
        Garrison = new TowerGeneratingGarrison(this, GenerationRate);
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

    void ITower.SetGarrisonCount(float newCount)
    {
        //Garrison.Count = newCount;
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

    public void Destroy()
    {
        Navigator.Destroy();
    }
}
