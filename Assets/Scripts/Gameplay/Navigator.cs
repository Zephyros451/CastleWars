using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class Navigator : MonoBehaviour
{
    [SerializeField] private List<Tower> towers;
    [SerializeField] private List<Path> paths;

    public List<Path> Paths => paths;
    public List<Tower> NotConnectedTowers
    {
        get
        {
            var temp = FindObjectsOfType<Tower>().Except(towers).Reverse().ToList();
            temp.Remove(GetComponent<Tower>());
            return temp;
        }
    }

    public BezierCurve GetPathTo(Tower tower)
    {
        if(towers.Contains(tower))
        {
            return paths[towers.IndexOf(tower)].GetCurveTo(tower);
        }
        return null;
    }

    public void RegisterPathTo(Tower tower, Path path)
    {
        if (towers.Contains(tower))
            return;

        towers.Add(tower);
        paths.Add(path);        
    }

    public void UnRegisterPath(Path path)
    {
        if (!paths.Contains(path))
            return;

        towers.Remove(towers[paths.IndexOf(path)]);
        paths.Remove(path);
    }

    public bool HasNeighbourWithAllegiance(Allegiance allegiance)
    {
        foreach(var tower in towers)
        {
            if (tower.Allegiance == allegiance)
                return true;
        }
        return false;
    }
}
