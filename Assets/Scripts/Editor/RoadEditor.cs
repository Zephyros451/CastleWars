using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
    Road road;

    private void OnSceneGUI()
    {
        if (road.AutoUpdate && Event.current.type == EventType.Repaint)
        {
            road.UpdateRoad();
        }
    }

    private void OnEnable()
    {
        road = (Road)target;
    }
}
