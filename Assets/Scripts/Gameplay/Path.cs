using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Tower> towers = new List<Tower>(2);
    [SerializeField] private List<BezierCurve> directions = new List<BezierCurve>(2);

    public void Initialize(Tower tower1, Tower tower2)
    {
        var curvePrefab = AssetDatabase.LoadAssetAtPath<BezierCurve>("Assets/Prefabs/Curve.prefab");

        var direction = PrefabUtility.InstantiatePrefab(curvePrefab, transform) as BezierCurve;
        direction.AddPointAt(tower2.transform.position);
        direction.AddPointAt(tower1.transform.position);
        towers.Add(tower1);
        directions.Add(direction);

        direction = PrefabUtility.InstantiatePrefab(curvePrefab, transform) as BezierCurve;
        
        direction.AddPointAt(tower1.transform.position);
        direction.AddPointAt(tower2.transform.position);
        towers.Add(tower2);
        directions.Add(direction);

        tower1.Navigator.RegisterPathTo(tower2, this);
        tower2.Navigator.RegisterPathTo(tower1, this);
    }

    public void Destroy()
    {
        for (int i = 0; i < towers.Count; i++) 
        {
            towers[i].Navigator.UnRegisterPath(this);
        }
    }

    public BezierCurve GetCurveTo(Tower tower)
    {
        return directions[towers.IndexOf(tower)];
    }
}
