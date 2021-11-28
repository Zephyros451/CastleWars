using UnityEngine;

[CreateAssetMenu(menuName = "Spawn Data/Unit Data")]
public class UnitSpawnData : ScriptableObject
{
    [SerializeField] private UnitSheetData unitData;
    public Unit UnitPrefab;
    public Model ModelPrefab;

    [Space]
    [SerializeField] private MeshRenderer[] levelSkins;

    public UnitData GetUnitData(int lvl)
    {
        return new UnitData(unitData.LevelData[lvl].hp,
            unitData.LevelData[lvl].attackInField,
            unitData.LevelData[lvl].attackInTower,
            unitData.LevelData[lvl].speed,
            unitData.LevelData[lvl].range,
            ModelPrefab);
    }

    public float GetUnitHP(int lvl)
    {
        return unitData.LevelData[lvl].hp;
    }

    public float GetUnitAttack(int lvl)
    {
        return unitData.LevelData[lvl].attackInField;
    }

    public float GetUnitSpeed(int lvl)
    {
        return unitData.LevelData[lvl].speed;
    }

    public MeshRenderer GetSkin(int level)
    {
        return levelSkins[level];
    }
}
