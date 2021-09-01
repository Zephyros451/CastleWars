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
    
    private int towerLevel = 1;
    private WaitForSeconds garrisonReplenishDeltaTime = new WaitForSeconds(1.2f);

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
            if (garrisonCount < towerLevel * 5 && towerLevel < 5)
            {
                lvlUp.interactable = false;
            }
            else
            {
                lvlUp.interactable = true;
            }

            garrisonCounterText.text = $"{(int)garrisonCount}/{towerLevel*10}";
        }
    }
    public int Level => towerLevel;
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
        lvlUp.interactable = false;
        GarrisonCount -= towerLevel * 5;
        towerLevel++;
        towerLevelText.text = towerLevel.ToString();

        if(towerLevel == 5)
        {
            lvlUp.enabled = false;
        }
    }

    private void ChangeAllegiance(Allegiance newAllegiance)
    {
        if (towerLevel > 1)
        {
            towerLevel--;
            towerLevelText.text = towerLevel.ToString();
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
            if (GarrisonCount < towerLevel * 10)
            {
                if (towerData.type != Type.A)
                {
                    GarrisonCount++;
                }
            }
            else if(GarrisonCount > towerLevel * 10 + 1)
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
                if (model.type == Type.LI || model.type == Type.A)
                {
                    GarrisonCount -= 0.8f;
                }
                else if (model.type == Type.HI)
                {
                    GarrisonCount -= 1.2f;
                }

                if (GarrisonCount <= 0)
                {
                    ChangeAllegiance(model.Allegiance);
                }
            }
            Destroy(model.gameObject);
        }
    }
}
