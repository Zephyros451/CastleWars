using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Model : MonoBehaviour
{
    [SerializeField, HideInInspector] private SphereCollider collider;

    [HideInInspector] public bool IsActive;
    [HideInInspector] public int SegmentsTravelled;

    private Unit unit;
    private float hp;

    public Allegiance Allegiance { get; private set; }
    public float Attack => unit.UnitSheetData.UnitLevelData[unit.Level].attackInField;

    public void Init(Unit unit, Allegiance allegiance)
    {
        Allegiance = allegiance;
        this.unit = unit;
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

    private void Reset()
    {
        collider = GetComponent<SphereCollider>();
    }
}
