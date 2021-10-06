using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

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
    [SerializeField] private TowerScaffoldingFlag towerScaffolding;
    [SerializeField] private List<Renderer> renderers;

    private void OnEnable()
    {
        Reset();

        if(tower.Allegiance != Allegiance.Player)
        {
            levelUp.enabled = false;
        }

        towerScaffolding.gameObject.SetActive(false);

        LevelUpButtonAnimation();
    }

    private void Start()
    {
        UpdateLevelWithoutAnimation();

        tower.TowerDataChanged += UpdateTower;
        tower.GarrisonCountChanged += UpdateGarrisonCount;
        tower.LevelReseted += UpdateLevelWithoutAnimation;
        tower.LevelUpStarted += OnLevelUpStarted;
        tower.LevelUpEnded += OnLevelUpEnded;
    }

    private void OnDestroy()
    {
        tower.TowerDataChanged -= UpdateTower;
        tower.GarrisonCountChanged -= UpdateGarrisonCount;
        tower.LevelReseted -= UpdateLevelWithoutAnimation;
        tower.LevelUpStarted -= OnLevelUpStarted;
        tower.LevelUpEnded -= OnLevelUpEnded;
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

    private void OnLevelUpStarted()
    {
        UpdateLevel();
        towerScaffolding.gameObject.SetActive(true);
    }

    private void OnLevelUpEnded()
    {
        towerScaffolding.gameObject.SetActive(false);
    }

    private void UpdateLevel()
    {
        levelCounterText.text = $"Lv. {(tower.Level + 1).ToString()}";
        garrisonCounterSlider.maxValue = tower.QuantityCap;
        SquashAnimation();
        UpdateGarrisonCount();
    }

    private void UpdateLevelWithoutAnimation()
    {
        levelCounterText.text = $"Lv. {(tower.Level + 1).ToString()}";
        garrisonCounterSlider.maxValue = tower.QuantityCap;
        UpdateGarrisonCount();
    }

    private void UpdateGarrisonCount()
    {
        garrisonCounterText.text = ((int)tower.GarrisonCount).ToString();
        garrisonCounterSlider.value = tower.GarrisonCount;

        if (tower.Allegiance == Allegiance.Player)
        {
            if (tower.GarrisonCount < tower.LvlUpQuantity)
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

    private void LevelUpButtonAnimation()
    {
        levelUp.transform.DOLocalJump(new Vector3(levelUp.transform.localPosition.x,1f,0f), 2f, 1, 0.75f).SetLoops(-1, LoopType.Restart);
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
        tower = transform.parent.GetComponent<Tower>();
        garrisonCounterBackground = tower.GetComponentInChildren<TowerGarrisonBackImageFlag>().GetComponent<Image>();
        garrisonCounterFront = tower.GetComponentInChildren<TowerGarrisonFrontImageFlag>().GetComponent<Image>();
        garrisonCounterText = tower.GetComponentInChildren<TowerGarrisonTextFlag>().GetComponent<TextMeshProUGUI>();
        garrisonCounterSlider = tower.GetComponentInChildren<Slider>();
        levelCounterBackground = tower.GetComponentInChildren<TowerLevelBackImageFlag>().GetComponent<Image>();
        levelCounterFront = tower.GetComponentInChildren<TowerLevelFrontImageFlag>().GetComponent<Image>();
        levelCounterText = tower.GetComponentInChildren<TowerLevelTextFlag>().GetComponent<TextMeshProUGUI>();
        levelUp = tower.GetComponentInChildren<TowerLevelUpImageFlag>().GetComponent<Image>();
        towerScaffolding = tower.GetComponentInChildren<TowerScaffoldingFlag>();
        renderers = new List<Renderer>(tower.GetComponentsInChildren<Renderer>());

        for (int i = renderers.Count - 1; i >= 0; i--) 
        {
            if(renderers[i].TryGetComponent(out TowerGroundAreaFlag groundFlag) ||
               renderers[i].TryGetComponent(out TowerScaffoldingFlag scaffoldingFlag))
            {
                renderers.Remove(renderers[i]);
            }
        }
    }
}
