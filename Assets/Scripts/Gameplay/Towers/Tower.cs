using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public event Action<TowerData> TowerDataChanged;
    public event Action BeingDestroyed;

    [SerializeField] private Allegiance allegiance;
    [SerializeField] private TowerType type;
    [SerializeField, HideInInspector] private TowerSheetData towerSheetData;
    [SerializeField, HideInInspector] private List<TowerData> towerDataInstances;
    [SerializeField, HideInInspector] private TowerData towerData;
    [SerializeField] private TowerMediator mediator;

    public Allegiance Allegiance => allegiance;
    public TowerType TowerType => type;
    public TowerSheetData TowerSheetData => towerSheetData;
    public TowerData TowerData => towerData;
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
    public void Initialize(TowerType newType, Allegiance newAllegiance)
    {
        Mediator.Reset();

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
public enum TowerType { Swordsman, Spearman, Archer }
