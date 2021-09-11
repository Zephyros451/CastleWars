using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tower))]
public class TowerEditor : Editor
{
    private Tower tower;

    public void OnEnable()
    {
        tower = (Tower)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (tower.IsNotInitialized || tower.Allegiance != tower.InitializedAllegiance || tower.TowerType != tower.InitializedTowerType)
        {
            GUILayout.Label("Tower is not initialized");
        }
        else
        {
            GUILayout.Label($"Initialized as {tower.InitializedAllegiance.ToString()} {tower.InitializedTowerType.ToString()}");
        }

        if (GUILayout.Button("Initialize"))
        {
            tower.Initialize(tower.TowerType, tower.Allegiance);
            tower.InitializedTowerType = tower.TowerType;
            tower.InitializedAllegiance = tower.Allegiance;
            EditorUtility.SetDirty(target);
        }
    }
}
