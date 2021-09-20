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

    public DirectionType GetDirectionTypeTo(Tower tower)
    {
        if(towers.Contains(tower))
        {
            return paths[towers.IndexOf(tower)].GetDirectionTypeTo(tower);
        }
        return DirectionType.None;
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

    public void Destroy()
    {
        foreach(var path in paths)
        {
            path.Destroy();
            DestroyImmediate(path);
        }
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

public enum DirectionType
{ Forward, Backward, None }
