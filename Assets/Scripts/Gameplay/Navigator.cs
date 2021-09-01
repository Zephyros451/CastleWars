using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class Navigator : MonoBehaviour
{
    [SerializeField] private Tower[] towers;
    [SerializeField] private BezierCurve[] curves;

    public BezierCurve GetPathTo(Tower tower)
    {
        if(towers.Contains(tower))
        {
            return curves[Array.IndexOf(towers, tower)];
        }
        return null;
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
