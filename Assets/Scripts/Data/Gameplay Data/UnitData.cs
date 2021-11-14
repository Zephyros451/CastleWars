public class UnitData
{
    private float hp;
    private float fieldAttack;
    private float towerAttack;
    private float speed;
    private float range;
    private Model modelPrefab;

    public float HP => hp;
    public float FieldAttack => fieldAttack;
    public float TowerAttack => towerAttack;
    public float Speed => speed;
    public float Range => range;
    public Model ModelPrefab => modelPrefab;

    public UnitData(float hp, float fieldAttack, float towerAttack, float speed, float range, Model modelPrefab)
    {
        this.hp = hp;
        this.fieldAttack = fieldAttack;
        this.towerAttack = towerAttack;
        this.speed = speed;
        this.range = range;
        this.modelPrefab = modelPrefab;
    }

    public UnitData(UnitData unitData)
    {
        this.hp = unitData.hp;
        this.fieldAttack = unitData.fieldAttack;
        this.towerAttack = unitData.towerAttack;
        this.speed = unitData.speed;
        this.range = unitData.range;
        this.modelPrefab = unitData.modelPrefab;
    }

    public void IncreaseHP(float amount)
    {
        this.hp += amount;
    }

    public void IncreaseAttack(float amount)
    {
        this.fieldAttack += amount;
    }

    public void DecreaseHP(float amount)
    {
        this.hp -= amount;
    }

    public void ApplyBuff(BuffData buffData)
    {
        this.hp += buffData.HP;
        this.fieldAttack += buffData.attack;
    }
}
