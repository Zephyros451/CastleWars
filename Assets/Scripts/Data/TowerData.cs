using UnityEngine;

[CreateAssetMenu(menuName ="Data/Tower Data")]
public class TowerData : ScriptableObject
{
    public UnitData unitData;
    public Tower[] levelPrefabs;
    public Material material;

    public Sprite backGarrisonUI;
    public Sprite frontGarrisonUI;
    public Sprite backLevelUI;
    public Sprite frontLevelUI;
}
