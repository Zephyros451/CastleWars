using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour, ITower
{
    public event Action<TowerData> TowerDataChanged;
    public event Action GarrisonCountChanged;
    public event Action LevelReseted;
    public event Action LevelUpStarted;
    public event Action LevelUpEnded;

    [SerializeField, HideInInspector] private Allegiance allegiance;
    [SerializeField, HideInInspector] private TowerType type;
    [SerializeField, HideInInspector] private TowerSheetData towerSheetData;
    [SerializeField, HideInInspector] private List<TowerData> towerDataInstances;
    [SerializeField, HideInInspector] private TowerData towerData;    

    [Space]
    [SerializeField, ReadOnly] private TowerCollision collision;
    [SerializeField, ReadOnly] private TowerTroopSender troopSender;
    [SerializeField, ReadOnly] private Navigator navigator;

    public Navigator Navigator => navigator;
    private TowerGarrison Garrison { get; set; }
    private TowerLevel level { get; set; }

    public int QuantityCap => towerSheetData.TowerLevelData[level.Value].quantityCap;
    public int LvlUpQuantity => towerSheetData.TowerLevelData[level.Value].lvlUpQuantity;
    public float GenerationRate => towerSheetData.TowerLevelData[level.Value].generationRate;
    public float LvlUpTime => towerSheetData.TowerLevelData[level.Value].lvlUpTime;
    public float AttackInTower => towerSheetData.TowerLevelData[level.Value].attackInTower;
    public float HP => towerSheetData.TowerLevelData[level.Value].hp;
    public Unit UnitPrefab => towerData.unitData.unitPrefab;
    public Model ModelPrefab => towerData.unitData.modelPrefab;
    public Allegiance Allegiance => allegiance;
    public TowerType TowerType => type;
    public TowerData TowerData => towerData;
    public float GarrisonCount => Garrison.Count;
    public bool IsNotUnderAttack => Garrison.IsNotUnderAttack;
    public int Level => level.Value;
    public bool IsNotLevelingUp => level.IsNotLevelingUp;


    private void Awake()
    {
        level = new TowerLevel(this, 0);
        Garrison = new TowerGarrison(this, towerSheetData);
    }

    private void Start()
    {
        TowerDataChanged?.Invoke(towerData);

        StartCoroutine(Garrison.GarrisonGeneration());
        StartCoroutine(Garrison.GarrisonDegeneration());

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

    private void OnTowerGotBackUp()
    {
        Garrison.OnAllyCame();
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

    public void SendTroopTo(Tower anotherTower)
    {
        troopSender.SendTroopTo(anotherTower);
    }

    public void ChangeAllegiance(Allegiance newAllegiance)
    {
        level.Reset();

        switch(newAllegiance)
        {
            case Allegiance.Player:
                towerData = towerDataInstances[0];
                break;
            case Allegiance.Neutral:
                towerData = towerDataInstances[1];
                break;
            case Allegiance.Enemy:
                towerData = towerDataInstances[2];
                break;
            default:
                Debug.Log("Wrong Allegiance Type");
                break;
        }
        
        allegiance = newAllegiance;
        
        level.IsNotLevelingUp = true;

        TowerDataChanged?.Invoke(towerData);
    }

    public void DecreaseGarrisonCount(int amount)
    {
        Garrison.DecreaseGarrisonCount(amount);
    }

    public void SetGarrisonCount(float newCount)
    {
        Garrison.SetGarrisonCount(newCount);
    }

    public void LevelUp()
    {
        level.LevelUp();
    }

#if UNITY_EDITOR
    public void Initialize(TowerType newType, Allegiance newAllegiance)
    {
        Reset();

        type = newType;
        allegiance = newAllegiance;

        string[] guids = AssetDatabase.FindAssets("t:TowerDataSettings");
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        towerDataInstances = new List<TowerData>(AssetDatabase.LoadAssetAtPath<TowerDataSettings>(path).GetData(type));

        switch (type)
        {
            case TowerType.Swordsman:
                guids = AssetDatabase.FindAssets("t:LITowerSheetData");
                break;
            case TowerType.Spearman:
                guids = AssetDatabase.FindAssets("t:HITowerSheetData");
                break;
            case TowerType.Archer:
                guids = AssetDatabase.FindAssets("t:ATowerSheetData");
                break;
            default:
                guids = new string[1];
                Debug.LogError("wrong tower type");
                break;
        }

        path = AssetDatabase.GUIDToAssetPath(guids[0]);
        towerSheetData = AssetDatabase.LoadAssetAtPath<TowerSheetData>(path);

        switch (allegiance)
        {
            case Allegiance.Player:
                towerData = towerDataInstances[0];
                break;
            case Allegiance.Neutral:
                towerData = towerDataInstances[1];
                break;
            case Allegiance.Enemy:
                towerData = towerDataInstances[2];
                break;
        }

        GetComponent<TowerView>().Initialize(towerData);
    }

    private void Reset()
    {        
        navigator = GetComponent<Navigator>();
        collision = GetComponentInChildren<TowerCollision>();
        troopSender = GetComponent<TowerTroopSender>();
    }

    private void Destroy()
    {
        Navigator.Destroy();
    }
#endif
}

public enum Allegiance { Player, Neutral, Enemy }

public enum TowerType { Swordsman, Spearman, Archer }
