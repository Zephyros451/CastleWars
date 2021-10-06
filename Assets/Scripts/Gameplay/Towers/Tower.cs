using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class Tower : MonoBehaviour
{
    public event Action<TowerData> TowerDataChanged;

    [SerializeField, HideInInspector] private Allegiance allegiance;
    [SerializeField, HideInInspector] private TowerType type;
    [SerializeField, HideInInspector] private TowerSheetData towerSheetData;
    [SerializeField, HideInInspector] private List<TowerData> towerDataInstances;
    [SerializeField, HideInInspector] private TowerData towerData;    

    [Space]
    [SerializeField, ReadOnly] private TowerCollision collision;
    [SerializeField, ReadOnly] private TowerTroopSender troopSender;
    [SerializeField, ReadOnly] private Navigator navigator;

    public TowerTroopSender TroopSender => troopSender;
    public Navigator Navigator => navigator;
    public TowerGarrison Garrison { get; private set; }
    public TowerLevel Level { get; private set; }
    public TowerCollision Collision => collision;

    public int QuantityCap => towerSheetData.TowerLevelData[Level.Value].quantityCap;
    public int LvlUpQuantity => towerSheetData.TowerLevelData[Level.Value].lvlUpQuantity;
    public float GenerationRate => towerSheetData.TowerLevelData[Level.Value].generationRate;
    public float LvlUpTime => towerSheetData.TowerLevelData[Level.Value].lvlUpTime;
    public float AttackInTower => towerSheetData.TowerLevelData[Level.Value].attackInTower;
    public float HP => towerSheetData.TowerLevelData[Level.Value].hp;
    public Unit UnitPrefab => towerData.unitData.unitPrefab;
    public Model ModelPrefab => towerData.unitData.modelPrefab;

    public Allegiance Allegiance => allegiance;
    public TowerType TowerType => type;
    public TowerData TowerData => towerData;

    private void Awake()
    {
        Level = new TowerLevel(this, 0);
        Garrison = new TowerGarrison(this, towerSheetData);
    }

    private void Start()
    {
        TowerDataChanged?.Invoke(towerData);

        StartCoroutine(Garrison.GarrisonGeneration());
        StartCoroutine(Garrison.GarrisonDegeneration());
    }

    public void ChangeAllegiance(Allegiance newAllegiance)
    {
        Level.Reset();

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
        
        Level.IsNotLevelingUp = true;

        TowerDataChanged?.Invoke(towerData);
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
