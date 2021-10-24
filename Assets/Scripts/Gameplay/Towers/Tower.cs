using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public event Action<TowerSpawnData> TowerDataChanged;
    public event Action BeingDestroyed;

    [SerializeField] private Allegiance allegiance;
    [SerializeField] private UnitType unitType;
    [SerializeField] private TowerType towerType;
    [SerializeField, HideInInspector] private TowerSheetData towerSheetData;
    [SerializeField, HideInInspector] private List<TowerSpawnData> towerDataInstances;
    [SerializeField, HideInInspector] private TowerSpawnData towerData;
    [SerializeField, HideInInspector] private TowerMediator mediator;

    public Allegiance Allegiance => allegiance;
    public UnitType TowerType => unitType;
    public TowerSheetData TowerSheetData => towerSheetData;
    public TowerSpawnData TowerData => towerData;
    public TowerMediator Mediator => mediator;

    private void Start()
    {
        TowerDataChanged?.Invoke(towerData);
    }    

    public void ChangeAllegiance(Allegiance newAllegiance)
    {
        switch (newAllegiance)
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
        TowerDataChanged?.Invoke(towerData);
    }

#if UNITY_EDITOR
    public void Initialize(UnitType newType, Allegiance newAllegiance)
    {
        Mediator.Reset();

        unitType = newType;
        allegiance = newAllegiance;

        string[] guids = AssetDatabase.FindAssets("t:TowerSpawnDataSettings");
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        towerDataInstances = new List<TowerSpawnData>(AssetDatabase.LoadAssetAtPath<TowerSpawnDataSettings>(path).GetData(unitType));

        switch (unitType)
        {
            case UnitType.Swordsman:
                guids = AssetDatabase.FindAssets("t:LITowerSheetData");
                break;
            case UnitType.Spearman:
                guids = AssetDatabase.FindAssets("t:HITowerSheetData");
                break;
            case UnitType.Archer:
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

        GetComponentInChildren<TowerView>().Initialize(towerData);
    }

    private void Reset()
    {
        mediator = GetComponent<TowerMediator>();
    }

    private void Destroy()
    {
        BeingDestroyed?.Invoke();
    }
#endif
}

public enum Allegiance { Player, Neutral, Enemy }
public enum UnitType { Swordsman, Spearman, Archer }
public enum TowerType { Generating, Buff}
