public struct UnitData
{
    private float hp;
    private float attack;
    private float speed;

    public float HP => hp;
    public float Attack => attack;
    public float Speed => speed;

    public UnitData(float hp, float attack, float speed)
    {
        this.hp = hp;
        this.attack = attack;
        this.speed = speed;
    }

    public void IncreaseHP(float amount)
    {
        this.hp += amount;
    }

    public void IncreaseAttack(float amount)
    {
        this.attack += amount;
    }
}
