using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Tower> towers = new List<Tower>(2);
    [SerializeField] private BezierCurve curve;

    private List<DirectionType> directions = new List<DirectionType> { DirectionType.Forward, DirectionType.Backward };

    public BezierCurve Curve => curve;

    public void Destroy()
    {
        for (int i = 0; i < towers.Count; i++) 
        {
            towers[i].Mediator.Navigator.UnRegisterPath(this);
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

    public Vector3 GetStartingPointTo(Tower tower)
    {
        switch(directions[towers.IndexOf(tower)])
        {
            case DirectionType.Forward:
                return curve.GetAnchorPoints()[0].position;
            case DirectionType.Backward:
                return curve.GetAnchorPoints()[curve.GetAnchorPoints().Length-1].position;
            default:
                return Vector3.zero;
        }
    }

#if UNITY_EDITOR
    public void Initialize(Tower tower1, Tower tower2)
    {
        var curvePrefab = AssetDatabase.LoadAssetAtPath<BezierCurve>("Assets/Prefabs/Curve.prefab");

        var newCurve = PrefabUtility.InstantiatePrefab(curvePrefab, transform) as BezierCurve;
        newCurve.AddPointAt(tower2.transform.position + (tower1.transform.position - tower2.transform.position).normalized);
        newCurve.AddPointAt(tower1.transform.position + (tower2.transform.position - tower1.transform.position).normalized);
        curve = newCurve;

        towers.Add(tower1);
        towers.Add(tower2);

        tower1.Mediator.Navigator.RegisterPathTo(tower2, this);
        tower2.Mediator.Navigator.RegisterPathTo(tower1, this);

        GetComponent<Road>().InitializeRoad();
    }
#endif
}
