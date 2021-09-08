using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerSheetData towerSheetData;
    [SerializeField] private TowerData towerData;
    [SerializeField] private AllegianceSettings allegianceSettings;

    [SerializeField] private TextMeshProUGUI garrisonCounterText;
    [SerializeField] private TextMeshProUGUI towerLevelText;
    [SerializeField] private Navigator navigator;
    [SerializeField] private Button lvlUp;

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
    public Allegiance Allegiance => towerData.Allegiance;

    private void Start()
    {
        StartCoroutine(GarrisonUpdate());
    }

    private void OnEnable()
    {
        GameState.instance.YouLose += Stop;
        GameState.instance.YouWin += Stop;
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
            case Allegiance.Enemy:
                towerData = allegianceSettings.Enemy;
                lvlUp.enabled = false;
                break;
            case Allegiance.Neutral:
                towerData = allegianceSettings.Neutral;
                lvlUp.enabled = false;
                break;
            case Allegiance.Player:
                towerData = allegianceSettings.Player;
                lvlUp.enabled = true;
                break;
            default:
                Debug.Log("Wrong Allegiance Type");
                break;
        }

        GetComponent<Renderer>().sharedMaterial = this.towerData.material;
    }

    private IEnumerator GarrisonUpdate()
    {
        while (true)
        {
            yield return garrisonReplenishDeltaTime;
            if (GarrisonCount < QuantityCap)
            {
                if (towerData.type != TowerType.A && shouldGenerate)
                {
                    GarrisonCount += GenerationRate;
                }
            }
            else if(GarrisonCount > QuantityCap + 1)
            {
                GarrisonCount -= GenerationRate;
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
            model.unit = unit;
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

    private void Stop()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Model model))
        {
            if (model.Allegiance == towerData.Allegiance)
            {
                GarrisonCount++;
            }
            else
            {
                if (replenishCooldownCoroutine != null)
                {
                    StopCoroutine(replenishCooldownCoroutine);
                }

                GarrisonCount -= model.unit.UnitSheetData.UnitLevelData[model.unit.Level].attackInField / towerSheetData.TowerLevelData[Level].hp;
                model.TakeDamage(towerSheetData.TowerLevelData[Level].attackInTower * GarrisonCount);

                if (GarrisonCount <= 0)
                {
                    ChangeAllegiance(model.Allegiance);
                }

                replenishCooldownCoroutine = StartCoroutine(ReplenishCooldown());
            }
        }
    }
}
