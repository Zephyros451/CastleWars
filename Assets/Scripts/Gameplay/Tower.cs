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

    private List<Unit> units = new List<Unit>();
    private int towerLevel = 0;
    private WaitForSeconds generationStopCooldown = new WaitForSeconds(2f);
    private WaitForSeconds generationRate;
    private WaitForSeconds degenerationRate;
    private int inactiveAttackersInTower = 0;

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

            if (garrisonCount < 0f) garrisonCount = 0f;

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
    public float AttackInTower => towerSheetData.TowerLevelData[Level].attackInTower;
    public float HP => towerSheetData.TowerLevelData[Level].hp;
    public bool IsNotUnderAttack { get; private set; } = true;

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

        generationRate = new WaitForSeconds(towerSheetData.TowerLevelData[Level].generationRate);
        degenerationRate = new WaitForSeconds(0.8f);
        
        StartCoroutine(GarrisonGeneration());
        StartCoroutine(GarrisonDegeneration());
    }

    public void LevelUp()
    {
        if (GarrisonCount < LvlUpQuantity)
            return;

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

    private IEnumerator GarrisonGeneration()
    {
        while (true)
        {
            yield return generationRate;
            if (GarrisonCount < QuantityCap)
            {
                if (IsNotUnderAttack)
                {
                    GarrisonCount++;
                }
            }
        }
    }

    private IEnumerator GarrisonDegeneration()
    {
        while(true)
        {
            yield return degenerationRate;            
            if ((GarrisonCount - QuantityCap) >= 1f)
            {
                GarrisonCount--;
            }
        }
    }


    public void SendTroopTo(Tower tower)
    {
        var path = navigator.GetPathTo(tower);
        var direction = navigator.GetDirectionTypeTo(tower);
        if (path == null)
            return;

        float newGarrisonCount = GarrisonCount / 2f;
        int troopSize = (int)(GarrisonCount - newGarrisonCount);

        Unit unit = null;
        for (int i = 0; i < units.Count; i++) 
        {
            if (units[i].Curve == path)
            {
                unit = units[i];
                break;
            }
        }
        if(unit == null)
        {
            unit = Instantiate(towerData.unitData.unitPrefab, transform.position, Quaternion.identity);
            unit.Init(path, tower, Level, direction);
            units.Add(unit);
        }

        List<Model> models = new List<Model>();

        for (int i = 0; i < troopSize; i++)
        {
            var model = Instantiate(unit.UnitData.modelPrefab, transform.position, Quaternion.identity, unit.transform);
            model.Init(unit, Allegiance);
            models.Add(model);
        }
        unit.AddModels(models);

        GarrisonCount = newGarrisonCount;
    }

    private IEnumerator ReplenishCooldown()
    {
        IsNotUnderAttack = false;
        yield return generationStopCooldown;
        IsNotUnderAttack = true;
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

            if (Mathf.Approximately(GarrisonCount, 0f))
            {
                ChangeAllegiance(model.Allegiance);
                replenishCooldownCoroutine = StartCoroutine(ReplenishCooldown());
                return;
            }

            int numberOfAttacksBeforeDeath = Mathf.Approximately(model.HP % AttackInTower, 0f) ?
                                             (int)(model.HP/AttackInTower) : (int)(model.HP/AttackInTower+1);
            GarrisonCount -= model.Attack / HP;
            Destroy(model.gameObject);

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
