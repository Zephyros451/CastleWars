using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Tower))]
public class TowerView : MonoBehaviour
{
    [SerializeField, HideInInspector] private Tower tower;

    [SerializeField] private Image garrisonCounterBackground;
    [SerializeField] private Image garrisonCounterFront;
    [SerializeField] private TextMeshProUGUI garrisonCounterText;
    [SerializeField] private Slider garrisonCounterSlider;
    [SerializeField] private Image levelCounterBackground;
    [SerializeField] private Image levelCounterFront;
    [SerializeField] private TextMeshProUGUI levelCounterText;
    //[SerializeField] private Button lvlUp;
    [SerializeField] private Renderer[] renderers;

    private void OnEnable()
    {
        Reset();
        UpdateLevel();

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

        garrisonCounterBackground.sprite = tower.TowerData.backGarrisonUI;
        garrisonCounterFront.sprite = tower.TowerData.frontGarrisonUI;
        levelCounterBackground.sprite = tower.TowerData.backLevelUI;
        levelCounterFront.sprite = tower.TowerData.frontLevelUI;
    }

    private void UpdateLevel()
    {
        levelCounterText.text = $"Lv. {(tower.Level + 1).ToString()}";

        garrisonCounterSlider.maxValue = tower.QuantityCap;

        SquashAnimation();
        UpdateGarrisonCount();
    }

    private void UpdateGarrisonCount()
    {
        garrisonCounterText.text = ((int)tower.GarrisonCount).ToString();
        garrisonCounterSlider.value = tower.GarrisonCount;

        if (tower.GarrisonCount < tower.LvlUpQuantity && tower.Level < 4 && tower.Allegiance == Allegiance.Player)
        {
            //lvlUp.interactable = false;
        }
        else
        {
            //lvlUp.interactable = true;
        }
    }

    private void SquashAnimation()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1.1f), 0.12f))
            .Append(transform.DOScale(new Vector3(0.9f, 1.1f, 0.9f), 0.15f))
            .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.05f));
    }

    private void Reset()
    {
        tower = GetComponent<Tower>();
        garrisonCounterBackground = GetComponentInChildren<TowerGarrisonBackImageFlag>().GetComponent<Image>();
        garrisonCounterFront = GetComponentInChildren<TowerGarrisonFrontImageFlag>().GetComponent<Image>();
        garrisonCounterText = GetComponentInChildren<TowerGarrisonTextFlag>().GetComponent<TextMeshProUGUI>();
        garrisonCounterSlider = GetComponentInChildren<Slider>();
        levelCounterBackground = GetComponentInChildren<TowerLevelBackImageFlag>().GetComponent<Image>();
        levelCounterFront = GetComponentInChildren<TowerLevelFrontImageFlag>().GetComponent<Image>();
        levelCounterText = GetComponentInChildren<TowerLevelTextFlag>().GetComponent<TextMeshProUGUI>();
        //lvlUp = GetComponentInChildren<LvlUpButtonFlag>().GetComponent<Button>();
        renderers = GetComponentsInChildren<Renderer>();
    }
}
