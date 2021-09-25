using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
    private Road road;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Regenerate Mesh"))
        {
            road.CreateRoad();
        }
    }

    private void OnEnable()
    {
        road = (Road)target;
    }
}
