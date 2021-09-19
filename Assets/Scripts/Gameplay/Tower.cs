using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Navigator))]
public class Tower : MonoBehaviour
{
    [SerializeField, HideInInspector] private Allegiance allegiance;
    [SerializeField, HideInInspector] private TowerType type;

    [SerializeField, HideInInspector] private Allegiance initializedAllegiance;
    [SerializeField, HideInInspector] private TowerType initializedTowerType;

    [SerializeField, HideInInspector] private TowerSheetData towerSheetData;
    [SerializeField, HideInInspector] private List<TowerData> towerDataInstances;
    [SerializeField, HideInInspector] private TowerData towerData;

    [SerializeField, HideInInspector] private TextMeshProUGUI garrisonCounterText;
    [SerializeField, HideInInspector] private TextMeshProUGUI towerLevelText;
    [SerializeField, HideInInspector] private Button lvlUp;
    [SerializeField, HideInInspector] private Navigator navigator;
    [SerializeField, HideInInspector] private TowerCollision towerCollision;
    [SerializeField, HideInInspector] private Renderer[] renderers;

    private bool shouldGenerate = true;
    private int towerLevel = 0;
    private WaitForSeconds garrisonReplenishDeltaTime = new WaitForSeconds(1f);
    private WaitForSeconds replenishCooldown = new WaitForSeconds(2f);

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
            if (garrisonCount <= LvlUpQuantity && Level < 4)
            {
                lvlUp.interactable = false;
            }
            else
            {
                lvlUp.interactable = true;
            }

            garrisonCounterText.text = $"{(int)garrisonCount}/{QuantityCap}";
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
            towerLevelText.text = (Level + 1).ToString();
            garrisonCounterText.text = $"{(int)garrisonCount}/{QuantityCap}";
        }
    }

    public int QuantityCap => towerSheetData.TowerLevelData[Level].quantityCap;
    public int LvlUpQuantity => towerSheetData.TowerLevelData[Level].lvlUpQuantity;
    public float GenerationRate => towerSheetData.TowerLevelData[Level].generationRate;
    public float LvlUpTime => towerSheetData.TowerLevelData[Level].lvlUpTime;

    public Navigator Navigator => navigator;
    public Allegiance Allegiance => allegiance;
    public TowerType TowerType => type;
    public bool IsNotInitialized => towerSheetData == null || towerData == null;

    public TowerType InitializedTowerType { get { return initializedTowerType; } set { initializedTowerType = value; } }
    public Allegiance InitializedAllegiance { get { return initializedAllegiance; } set { initializedAllegiance = value; } }

    private void Start()
    {
        if(Allegiance == Allegiance.Player)
        {
            lvlUp.enabled = true;
        }
    }

    private void OnEnable()
    {
        GameState.instance.YouLose += Stop;
        GameState.instance.YouWin += Stop;
        towerCollision.TowerAttacked += OnTowerAttacked;
    }

    private void OnDisable()
    {
        GameState.instance.YouLose -= Stop;
        GameState.instance.YouWin -= Stop;
    }

    public void LevelUp()
    {
        GarrisonCount -= LvlUpQuantity;
        Level++;

        if(Level == 4)
        {
            lvlUp.enabled = false;
        }
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
                lvlUp.enabled = true;
                break;
            case Allegiance.Neutral:
                towerData = towerDataInstances[1];
                lvlUp.enabled = false;
                break;
            case Allegiance.Enemy:
                towerData = towerDataInstances[2];
                lvlUp.enabled = false;
                break;
            default:
                Debug.Log("Wrong Allegiance Type");
                break;
        }


        allegiance = newAllegiance;
        foreach (var renderer in renderers)
        {
            renderer.sharedMaterial = towerData.material;
        }
    }

    public void OnUpdateGarrison()
    {
        if (GarrisonCount < QuantityCap)
        {
            if (shouldGenerate)
            {
                GarrisonCount += GenerationRate;
            }
        }
        else if ((GarrisonCount - QuantityCap) > 1f)
        {
            GarrisonCount--;
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
        yield return replenishCooldown;
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

    private void Stop()
    {
        StopAllCoroutines();
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

        foreach(var renderer in renderers)
        {
            renderer.sharedMaterial = towerData.material;
        }
    }

    private void Reset()
    {
        garrisonCounterText = GetComponentInChildren<GarrisonCountFlag>().GetComponent<TextMeshProUGUI>();
        towerLevelText = GetComponentInChildren<TowerLevelText>().GetComponent<TextMeshProUGUI>();
        lvlUp = GetComponentInChildren<LvlUpButtonFlag>().GetComponent<Button>();
        navigator = GetComponent<Navigator>();
        towerCollision = GetComponentInChildren<TowerCollision>();
        renderers = GetComponentsInChildren<Renderer>();
    }
#endif

    private void Destroy()
    {
        Navigator.Destroy();
    }
}


public enum Allegiance { Player, Neutral, Enemy }

public enum TowerType { Swordsman, Spearman, Archer }
