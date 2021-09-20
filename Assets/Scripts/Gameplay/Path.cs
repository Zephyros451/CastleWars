using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Tower> towers = new List<Tower>(2);
    [SerializeField] private BezierCurve curve;

    private List<DirectionType> directions = new List<DirectionType> { DirectionType.Forward, DirectionType.Backward };

    public void Destroy()
    {
        for (int i = 0; i < towers.Count; i++) 
        {
            towers[i].Navigator.UnRegisterPath(this);
        }
    }

    public BezierCurve GetCurveTo(Tower tower)
    {
        return curve;
    }

    public DirectionType GetDirectionTypeTo(Tower tower)
    {
        return directions[towers.IndexOf(tower)];
    }

#if UNITY_EDITOR
    public void Initialize(Tower tower1, Tower tower2)
    {
        var curvePrefab = AssetDatabase.LoadAssetAtPath<BezierCurve>("Assets/Prefabs/Curve.prefab");

        var newCurve = PrefabUtility.InstantiatePrefab(curvePrefab, transform) as BezierCurve;
        newCurve.AddPointAt(tower2.transform.position);
        newCurve.AddPointAt(tower1.transform.position);
        curve = newCurve;

        towers.Add(tower1);
        towers.Add(tower2);

        tower1.Navigator.RegisterPathTo(tower2, this);
        tower2.Navigator.RegisterPathTo(tower1, this);
    }

#endif
}
