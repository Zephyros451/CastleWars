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
    [SerializeField] private Image levelUp;
    [SerializeField] private Renderer[] renderers;

    private void OnEnable()
    {
        Reset();
        UpdateLevel(false);

        if(tower.Allegiance != Allegiance.Player)
        {
            levelUp.enabled = false;
        }

        tower.TowerDataChanged += UpdateTower;
        tower.LevelChanged += OnLevelChanged;
        tower.GarrisonCountChanged += UpdateGarrisonCount;
    }

    private void OnDisable()
    {
        tower.TowerDataChanged -= UpdateTower;
        tower.LevelChanged -= OnLevelChanged;
        tower.GarrisonCountChanged -= UpdateGarrisonCount;
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

    private void OnLevelChanged()
    {
        UpdateLevel();
    }

    private void UpdateLevel(bool withAnimation = true)
    {
        levelCounterText.text = $"Lv. {(tower.Level + 1).ToString()}";

        garrisonCounterSlider.maxValue = tower.QuantityCap;

        if (withAnimation)
        {
            SquashAnimation();
        }

        UpdateGarrisonCount();
    }

    private void UpdateGarrisonCount()
    {
        garrisonCounterText.text = ((int)tower.GarrisonCount).ToString();
        garrisonCounterSlider.value = tower.GarrisonCount;

        if (tower.Allegiance == Allegiance.Player)
        {
            if (tower.GarrisonCount < tower.LvlUpQuantity && tower.Level < 4)
            {
                levelUp.enabled = false;
            }
            else
            {
                levelUp.enabled = true;
            }
        }
    }

    private void SquashAnimation()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1.1f), 0.12f))
            .Append(transform.DOScale(new Vector3(0.9f, 1.1f, 0.9f), 0.15f))
            .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.05f));
    }

#if UNITY_EDITOR
    public void Initialize(TowerData data)
    {
        Reset();
        UpdateTower(data);
    }
#endif

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
        levelUp = GetComponentInChildren<TowerLevelUpImageFlag>().GetComponent<Image>();
        renderers = GetComponentsInChildren<Renderer>();
    }
}
