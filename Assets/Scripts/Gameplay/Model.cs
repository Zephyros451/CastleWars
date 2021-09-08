using UnityEngine;

public class Model : MonoBehaviour
{
    [SerializeField] private SphereCollider collider;

    public bool IsActive;
    public int SegmentsTravelled;
    public Allegiance Allegiance;
    public TowerType type;
    public Unit unit;

    private float hp;

    private void Start()
    {
        hp = unit.UnitSheetData.UnitLevelData[unit.Level].hp;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp < 0f) 
        {
            Destroy(this.gameObject);
        }
    }

    public void ActivateCollider()
    {
        collider.enabled = true;
    }
}
