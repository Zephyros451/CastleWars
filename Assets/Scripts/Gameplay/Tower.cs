using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI garrisonCounterText;
    [SerializeField] private TextMeshProUGUI towerLevelText;
    [SerializeField] private TowerData towerData;
    [SerializeField] private Navigator navigator;
    [SerializeField] private Button lvlUp;
    [SerializeField] private AllegianceSettings allegianceSettings;

    private bool shouldGenerate = true;
    private int towerLevel = 1;
    private WaitForSeconds garrisonReplenishDeltaTime = new WaitForSeconds(1.2f);
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
            if (garrisonCount <= Level * 5 && Level < 5)
            {
                lvlUp.interactable = false;
            }
            else
            {
                lvlUp.interactable = true;
            }

            garrisonCounterText.text = $"{(int)garrisonCount}/{Level * 10}";
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
            towerLevelText.text = Level.ToString();
            garrisonCounterText.text = $"{(int)garrisonCount}/{towerLevel * 10}";
        }
    }

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
        GarrisonCount -= Level * 5;
        Level++;

        if(Level == 5)
        {
            lvlUp.enabled = false;
        }
    }

    private void ChangeAllegiance(Allegiance newAllegiance)
    {
        if (Level > 1)
        {
            Level = 1;
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
            if (GarrisonCount < Level * 10)
            {
                if (towerData.type != TowerType.A && shouldGenerate)
                {
                    GarrisonCount++;
                }
            }
            else if(GarrisonCount > Level * 10 + 1)
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
            models.Add(model);
        }

        unit.Init(path, models, tower);

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

                if (model.type == TowerType.LI || model.type == TowerType.A)
                {
                    GarrisonCount -= 0.5f;
                }
                else if (model.type == TowerType.HI)
                {
                    GarrisonCount -= 1.1f;
                }

                if (GarrisonCount <= 0)
                {
                    ChangeAllegiance(model.Allegiance);
                }

                replenishCooldownCoroutine = StartCoroutine(ReplenishCooldown());
            }
            Destroy(model.gameObject);
        }
    }
}
