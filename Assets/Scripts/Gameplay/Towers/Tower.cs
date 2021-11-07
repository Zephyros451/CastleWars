using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public event Action<TowerSpawnData> TowerDataChanged;
    public event Action BeingDestroyed;

    [SerializeField] private Allegiance allegiance;
    [SerializeField] private TowerType towerType;
    [SerializeField, HideInInspector] private TowerSheetData towerSheetData;
    [SerializeField, HideInInspector] private List<TowerSpawnData> towerDataInstances;
    [SerializeField, HideInInspector] private TowerSpawnData towerData;
    [SerializeField, HideInInspector] private TowerMediator mediator;

    public Allegiance Allegiance => allegiance;
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
    public void Initialize(Allegiance allegiance, TowerType towerType)
    {
        Mediator.Reset();

        this.towerType = towerType;
        this.allegiance = allegiance;

        string[] guids = AssetDatabase.FindAssets("t:TowerSpawnDataSettings");
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        towerDataInstances = new List<TowerSpawnData>(AssetDatabase.LoadAssetAtPath<TowerSpawnDataSettings>(path).GetData(towerType));

        switch (towerType)
        {
            case TowerType.SwordsmanGenerating:
                guids = AssetDatabase.FindAssets("t:LITowerSheetData");
                break;
            case TowerType.ArcherGenerating:
                guids = AssetDatabase.FindAssets("t:ATowerSheetData");
                break;
            case TowerType.AttackBuff:
                guids = AssetDatabase.FindAssets("t:");
                break;
            case TowerType.HPBuff:
                guids = AssetDatabase.FindAssets("t:");
                break;
            default:
                guids = new string[1];
                Debug.LogError("wrong tower type");
                break;
        }

        path = AssetDatabase.GUIDToAssetPath(guids[0]);
        towerSheetData = AssetDatabase.LoadAssetAtPath<TowerSheetData>(path);

        switch (this.allegiance)
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
public enum TowerType { SwordsmanGenerating, ArcherGenerating, AttackBuff, HPBuff }
