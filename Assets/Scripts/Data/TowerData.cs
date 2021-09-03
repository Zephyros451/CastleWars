using UnityEngine;

[CreateAssetMenu(menuName ="Data/Tower Data")]
public class TowerData : ScriptableObject
{
    public UnitData unitData;
    public Allegiance Allegiance;
    public TowerType type;
    public Tower[] levelPrefabs;
    public Material material;
}

public enum Allegiance { Player, Enemy, Neutral }

public enum TowerType { LI, HI, A }
