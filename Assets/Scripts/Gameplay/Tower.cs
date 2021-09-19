using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class Tower : MonoBehaviour
{
    public event Action<TowerData> TowerDataChanged;
    public event Action LevelChanged;
    public event Action GarrisonCountChanged;

    [SerializeField, HideInInspector] private Allegiance allegiance;
    [SerializeField, HideInInspector] private TowerType type;

    [SerializeField, HideInInspector] private TowerSheetData towerSheetData;
    [SerializeField, HideInInspector] private List<TowerData> towerDataInstances;
    [SerializeField, HideInInspector] private TowerData towerData;    
    [SerializeField, HideInInspector] private Navigator navigator;
    [SerializeField, HideInInspector] private TowerCollision towerCollision;

    private bool shouldGenerate = true;
    private int towerLevel = 0;
    private WaitForSeconds generationStopCooldown = new WaitForSeconds(2f);
    private WaitForSeconds generationRate;

    private Coroutine replenishCooldownCoroutine;

    private float garrisonCount = 10;
    public float GarrisonCount
    {
        get
        {
            return garrisonCount;
        }
        set
        {
            garrisonCount = value;
            GarrisonCountChanged?.Invoke();
        }
    }

    public int Level
    {
        get
        {
            return towerLevel;
        }
        private set
        {
            towerLevel = value;
            LevelChanged?.Invoke();
        }
    }

    public int QuantityCap => towerSheetData.TowerLevelData[Level].quantityCap;
    public int LvlUpQuantity => towerSheetData.TowerLevelData[Level].lvlUpQuantity;
    public float GenerationRate => towerSheetData.TowerLevelData[Level].generationRate;
    public float LvlUpTime => towerSheetData.TowerLevelData[Level].lvlUpTime;

    public Navigator Navigator => navigator;
    public Allegiance Allegiance => allegiance;
    public TowerType TowerType => type;

    private void OnEnable()
    {
        towerCollision.TowerAttacked += OnTowerAttacked;
    }

    private void OnDisable()
    {
        towerCollision.TowerAttacked -= OnTowerAttacked;
    }

    private void Start()
    {
        TowerDataChanged?.Invoke(towerData);

        if (Mathf.Approximately(towerSheetData.TowerLevelData[Level].generationRate, 0f))
            return;

        generationRate = new WaitForSeconds(towerSheetData.TowerLevelData[Level].generationRate);
        StartCoroutine(GarrisonUpdate());
    }

    public void LevelUp()
    {
        GarrisonCount -= LvlUpQuantity;
        Level++;
    }

    private void ChangeAllegiance(Allegiance newAllegiance)
    {
        if (Level > 0)
        {
            Level = 0;
        }

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

        TowerDataChanged?.Invoke(towerData);
    }

    public IEnumerator GarrisonUpdate()
    {
        while (true)
        {
            yield return generationRate;
            if (GarrisonCount < QuantityCap)
            {
                if (shouldGenerate)
                {
                    GarrisonCount++;
                }
            }
            else if ((GarrisonCount - QuantityCap) > 1f)
            {
                GarrisonCount--;
            }
        }
    }

    public void SendTroopTo(Tower tower)
    {
        var path = navigator.GetPathTo(tower);
        if (path == null)
            return;

        float newGarrisonCount = GarrisonCount / 2f;
        int troopSize = (int)(GarrisonCount - newGarrisonCount);

        var unit = Instantiate(towerData.unitData.unitPrefab, transform.position, Quaternion.identity);
        List<Model> models = new List<Model>();

        for (int i = 0; i < troopSize; i++)
        {
            var model = Instantiate(unit.UnitData.modelPrefab, transform.position, Quaternion.identity, unit.transform);
            model.Init(unit, Allegiance);
            models.Add(model);
        }

        unit.Init(path, models, tower, Level);

        GarrisonCount = newGarrisonCount;
    }

    private IEnumerator ReplenishCooldown()
    {
        shouldGenerate = false;
        yield return generationStopCooldown;
        shouldGenerate = true;
    }

    private void OnTowerAttacked(Model model)
    {
        if (model.Allegiance == Allegiance)
        {
            GarrisonCount++;
            Destroy(model.gameObject);
        }
        else
        {
            if (replenishCooldownCoroutine != null)
            {
                StopCoroutine(replenishCooldownCoroutine);
            }

            if (GarrisonCount <= 0)
            {
                ChangeAllegiance(model.Allegiance);
                replenishCooldownCoroutine = StartCoroutine(ReplenishCooldown());
                return;
            }

            GarrisonCount -= model.Attack / towerSheetData.TowerLevelData[Level].hp;
            model.TakeDamage(towerSheetData.TowerLevelData[Level].attackInTower * GarrisonCount);            

            replenishCooldownCoroutine = StartCoroutine(ReplenishCooldown());
        }
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
        towerCollision = GetComponentInChildren<TowerCollision>();
    }

    private void Destroy()
    {
        Navigator.Destroy();
    }
#endif
}

public enum Allegiance { Player, Neutral, Enemy }

public enum TowerType { Swordsman, Spearman, Archer }
