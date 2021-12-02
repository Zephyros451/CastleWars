using System.Collections.Generic;
using UnityEngine;

public class TowerTroopSender : MonoBehaviour
{
    [SerializeField] private Tower tower;

    private List<Unit> units = new List<Unit>();

    private ITower Tower => tower.Mediator;

    public void SendTroopTo(ITower anotherTower)
    {
        if (!Tower.IsNotLevelingUp)
            return;

        var path = Tower.Navigator.GetPathTo(anotherTower.Tower);
        var direction = Tower.Navigator.GetDirectionTypeTo(anotherTower.Tower);
        if (path == null)
            return;

        int troopSize = (int)(Tower.GarrisonCount / 2f);

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
            unit.Init(path, direction);
            units.Add(unit);
        }

        Stack<Model> models = new Stack<Model>();
        Stack<UnitData> unitsData = Tower.PopFromGarrison(troopSize);

        for (int i = 0; i < troopSize; i++)
        {
            var model = Instantiate(unitsData.Peek().ModelPrefab, Tower.Navigator.GetStartingPointTo(anotherTower.Tower),
                                    Quaternion.identity, unit.transform);
            
            model.Init(unitsData.Pop(), tower.Allegiance, Tower.Level, anotherTower);
            models.Push(model);
        }
        unit.AddModels(models);
    }

    private void Reset()
    {
        tower = GetComponent<Tower>();
    }
}
