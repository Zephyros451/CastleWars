using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class AIBrain : MonoBehaviour
{
    [SerializeField] private List<Tower> towers;    
    [SerializeField] private DifficultySettings difficultySettings;

    private List<Tower> aiTowers = new List<Tower>();
    private HashSet<Tower> nonAICloseTowers = new HashSet<Tower>();
    private GameDifficulty gameDifficulty;

    private void Start()
    {
        towers = FindObjectsOfType<Tower>().ToList();
        UpdateTowers();
        StartCoroutine(AIProcessing());
        gameDifficulty = difficultySettings.difficulty;
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

    private IEnumerator AIProcessing()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            UpdateTowers();
            foreach (var tower in aiTowers)
            {
                if (ShouldLevelUp(tower))
                {
                    tower.Mediator.LevelUp();
                }

                foreach (var closeTower in nonAICloseTowers)
                {
                    if (ShouldAttack(tower, closeTower))
                    {
                        tower.Mediator.SendTroopTo(closeTower.Mediator);
                    }
                }
            }
        }
    }

    private void UpdateTowers()
    {
        aiTowers.Clear();
        nonAICloseTowers.Clear();

        foreach (var tower in towers)
        {
            if (tower.Allegiance == Allegiance.Enemy)
            {
                aiTowers.Add(tower);
                continue;
            }

            if (tower.Mediator.Navigator.HasNeighbourWithAllegiance(Allegiance.Enemy))
            {
                nonAICloseTowers.Add(tower);
                continue;
            }
        }

        if (aiTowers.Count == 0)
        {
            GameState.instance.RaiseYouWin();
            return;
        }
        if (nonAICloseTowers.Count == 0)
        {
            GameState.instance.RaiseYouLose();
            return;
        }
    }

    private bool ShouldLevelUp(Tower tower)
    {
        if (tower.Mediator.Level == 4)
            return false;

        switch (gameDifficulty)
        {
            case GameDifficulty.Hard:
                if (tower.Mediator.Navigator.HasNeighbourWithAllegiance(Allegiance.Player))
                {
                    if (tower.Mediator.GarrisonCount >= tower.Mediator.QuantityCap)
                        return true;
                }
                else
                {
                    if (tower.Mediator.GarrisonCount >= tower.Mediator.LvlUpQuantity)
                        return true;
                }
                return false;

            case GameDifficulty.Normal:
                if (tower.Mediator.GarrisonCount >= tower.Mediator.QuantityCap * 0.75f)
                {
                    return true;
                }
                return false;

            case GameDifficulty.Easy:
                if (tower.Mediator.Navigator.HasNeighbourWithAllegiance(Allegiance.Player))
                {
                    if (tower.Mediator.GarrisonCount >= tower.Mediator.LvlUpQuantity)
                        return true;
                }
                else
                {
                    if (tower.Mediator.GarrisonCount == tower.Mediator.QuantityCap)
                        return true;
                }
                return false;

            default:
                return false;
        }
    }

    private bool ShouldAttack(Tower aiTower, Tower nonAITower)
    {
        if (!aiTower.Mediator.IsNotUnderAttack)
            return false;

        int random = Random.Range(0, 101);
        switch (gameDifficulty)
        {
            case GameDifficulty.Hard:
                if (random <= 30 && (aiTower.Mediator.GarrisonCount * 0.5f - nonAITower.Mediator.GarrisonCount) > aiTower.Mediator.GarrisonCount * 0.2f)
                    return true;
                if ((aiTower.Mediator.GarrisonCount * 0.5f - nonAITower.Mediator.GarrisonCount) > aiTower.Mediator.GarrisonCount * 0.5f)
                    return true;
                return false;
            case GameDifficulty.Normal:
                if (random <= 30 && (aiTower.Mediator.GarrisonCount * 0.5f - nonAITower.Mediator.GarrisonCount) > aiTower.Mediator.GarrisonCount * 0.1f)
                    return true;
                if ((aiTower.Mediator.GarrisonCount * 0.5f - nonAITower.Mediator.GarrisonCount) > aiTower.Mediator.GarrisonCount * 0.25f)
                    return true;
                return false;
            case GameDifficulty.Easy:
                if (random <= 30 && (aiTower.Mediator.GarrisonCount * 0.5f - nonAITower.Mediator.GarrisonCount) > aiTower.Mediator.GarrisonCount * 0.01f)
                    return true;
                if ((aiTower.Mediator.GarrisonCount * 0.5f - nonAITower.Mediator.GarrisonCount) > aiTower.Mediator.GarrisonCount * 0.1f)
                    return true;
                return false;
            default:
                return false;
        }
    }

    private void Stop()
    {
        StopAllCoroutines();
    }

    private void Reset()
    {
        difficultySettings = Resources.FindObjectsOfTypeAll<DifficultySettings>()[0];
    }

    public enum GameDifficulty { Easy, Normal, Hard }
}
