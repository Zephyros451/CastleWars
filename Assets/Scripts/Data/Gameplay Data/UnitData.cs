public struct UnitData
{
    private float hp;
    private float fieldAttack;
    private float towerAttack;
    private float speed;

    public float HP => hp;
    public float FieldAttack => fieldAttack;
    public float TowerAttack => towerAttack;
    public float Speed => speed;

    public UnitData(float hp, float fieldAttack, float towerAttack, float speed)
    {
        this.hp = hp;
        this.fieldAttack = fieldAttack;
        this.towerAttack = towerAttack;
        this.speed = speed;
    }

    public UnitData(UnitData unitData)
    {
        this.hp = unitData.hp;
        this.fieldAttack = unitData.fieldAttack;
        this.towerAttack = unitData.towerAttack;
        this.speed = unitData.speed;
    }

    public void IncreaseHP(float amount)
    {
        this.hp += amount;
    }

    public void IncreaseAttack(float amount)
    {
        this.fieldAttack += amount;
    }

    public void ApplyBuff(BuffData buffData)
    {
        this.hp += buffData.hp;
        this.fieldAttack += buffData.attack;
    }
}
