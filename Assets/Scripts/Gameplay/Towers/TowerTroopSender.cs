using System.Collections.Generic;
using UnityEngine;

public class TowerTroopSender : MonoBehaviour
{
    [SerializeField] private Tower tower;

    private List<Unit> units = new List<Unit>();
    private int inactiveAttackersInTower = 0;

    private TowerMediator Tower => tower.Mediator;

    public void SendTroopTo(ITower anotherTower)
    {
        if (!Tower.IsNotLevelingUp)
            return;

        var path = Tower.Navigator.GetPathTo(anotherTower.Tower);
        var direction = Tower.Navigator.GetDirectionTypeTo(anotherTower.Tower);
        if (path == null)
            return;

        float newGarrisonCount = Tower.GarrisonCount / 2f;
        int troopSize = (int)(Tower.GarrisonCount - newGarrisonCount);
        ((ITower)Tower).SetGarrisonCount(newGarrisonCount);

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
            unit = Instantiate(Tower.UnitPrefab, transform.position, Quaternion.identity);
            unit.Init(path, Tower, direction);
            units.Add(unit);
        }

        List<Model> models = new List<Model>();

        for (int i = 0; i < troopSize; i++)
        {
            var model = Instantiate(Tower.ModelPrefab, Tower.Navigator.GetStartingPointTo(anotherTower.Tower),
                                    Quaternion.identity, unit.transform);
            model.Init(unit, tower.Allegiance, Tower.Level);
            models.Add(model);
        }
        unit.AddModels(models);
    }

    private void Reset()
    {
        tower = GetComponent<Tower>();
    }
}
