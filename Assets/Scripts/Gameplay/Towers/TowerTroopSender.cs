using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerTroopSender : MonoBehaviour
{
    [SerializeField] private Tower tower;

    private List<Unit> units = new List<Unit>();
    private int inactiveAttackersInTower = 0;

    public void SendTroopTo(Tower anotherTower)
    {
        if (!tower.IsNotLevelingUp)
            return;

        var path = tower.Navigator.GetPathTo(anotherTower);
        var direction = tower.Navigator.GetDirectionTypeTo(anotherTower);
        if (path == null)
            return;

        float newGarrisonCount = tower.GarrisonCount / 2f;
        int troopSize = (int)(tower.GarrisonCount - newGarrisonCount);
        tower.SetGarrisonCount(newGarrisonCount);

        Unit unit = null;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].Curve == path)
            {
                unit = units[i];
                break;
            }
        }
        if (unit == null)
        {
            unit = Instantiate(tower.UnitPrefab, transform.position, Quaternion.identity);
            unit.Init(path, tower, direction);
            units.Add(unit);
        }

        List<Model> models = new List<Model>();

        for (int i = 0; i < troopSize; i++)
        {
            var model = Instantiate(tower.ModelPrefab, tower.Navigator.GetStartingPointTo(anotherTower),
                                    Quaternion.identity, unit.transform);
            model.Init(unit, tower.Allegiance, tower.Level);
            models.Add(model);
        }
        unit.AddModels(models);
    }

    private void Reset()
    {
        tower = GetComponent<Tower>();
    }
}
