using UnityEngine;

[CreateAssetMenu(menuName ="Spawn Data/Tower Data")]
public class TowerSpawnData : ScriptableObject
{
    public UnitSpawnData unitData;
    public Renderer[] levelPrefabs;
    public Material[] materials;

    [Space]
    public Sprite backGarrisonUI;
    public Sprite frontGarrisonUI;
    public Sprite backLevelUI;
    public Sprite frontLevelUI;
}
