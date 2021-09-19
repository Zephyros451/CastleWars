using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Tower))]
public class TowerView : MonoBehaviour
{
    [SerializeField, HideInInspector] private Tower tower;

    [SerializeField] private TextMeshProUGUI garrisonCounterText;
    [SerializeField] private TextMeshProUGUI towerLevelText;
    [SerializeField] private Button lvlUp;
    [SerializeField] private Renderer[] renderers;

    private void Start()
    {
        if (tower.Allegiance == Allegiance.Player)
        {
            lvlUp.enabled = true;
        }
    }

    private void OnEnable()
    {
        tower.TowerDataChanged += UpdateTower;
        tower.LevelChanged += UpdateLevel;
        tower.GarrisonCountChanged += UpdateGarrisonCount;
    }

    private void OnDisable()
    {
        tower.TowerDataChanged -= UpdateTower;
        tower.LevelChanged -= UpdateLevel;
        tower.GarrisonCountChanged -= UpdateGarrisonCount;
    }

    public void Initialize(TowerData data)
    {
        Reset();
        UpdateTower(data);
    }

    private void UpdateTower(TowerData data)
    {
        foreach (var renderer in renderers)
        {
            renderer.sharedMaterial = data.material;
        }

        if (tower.Allegiance == Allegiance.Player && tower.Level < 4)
        {
            lvlUp.enabled = true;
        }
        else
        {
            lvlUp.enabled = false;
        }
    }

    private void UpdateLevel()
    {
        towerLevelText.text = (tower.Level + 1).ToString();
        garrisonCounterText.text = $"{(int)tower.GarrisonCount}/{tower.QuantityCap}";

        if (tower.Level == 4)
        {
            lvlUp.enabled = false;
        }
    }

    private void UpdateGarrisonCount()
    {
        garrisonCounterText.text = $"{(int)tower.GarrisonCount}/{tower.QuantityCap}";

        if (tower.GarrisonCount <= tower.LvlUpQuantity && tower.Level < 4 && tower.Allegiance == Allegiance.Player)
        {
            lvlUp.interactable = false;
        }
        else
        {
            lvlUp.interactable = true;
        }
    }

    private void Reset()
    {
        tower = GetComponent<Tower>();
        garrisonCounterText = GetComponentInChildren<GarrisonCountFlag>().GetComponent<TextMeshProUGUI>();
        towerLevelText = GetComponentInChildren<TowerLevelText>().GetComponent<TextMeshProUGUI>();
        lvlUp = GetComponentInChildren<LvlUpButtonFlag>().GetComponent<Button>();
        renderers = GetComponentsInChildren<Renderer>();
    }
}
