using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerTroopSender : MonoBehaviour
{
    public Action CountChanged;

    [SerializeField] private Tower tower;

    private List<Unit> units = new List<Unit>();
    private int inactiveAttackersInTower = 0;

    public void SendTroopTo(Tower anotherTower)
    {
        if (!tower.Level.IsNotLevelingUp)
            return;

        var path = tower.Navigator.GetPathTo(anotherTower);
        var direction = tower.Navigator.GetDirectionTypeTo(anotherTower);
        if (path == null)
            return;

        float newGarrisonCount = tower.Garrison.Count / 2f;
        int troopSize = (int)(tower.Garrison.Count - newGarrisonCount);

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
            model.Init(unit, tower.Allegiance, tower.Level.Value);
            models.Add(model);
        }
        unit.AddModels(models);

        tower.Garrison.Count = newGarrisonCount;
    }

    private void Reset()
    {
        tower = GetComponent<Tower>();
    }
}
