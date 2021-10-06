using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Model : MonoBehaviour, IModel
{
    [SerializeField, HideInInspector] private SphereCollider collider;
    [SerializeField, HideInInspector] private Transform view;

    [HideInInspector] public bool IsActive;
    [HideInInspector] public int SegmentsTravelled;
    [HideInInspector] public int Level;

    private Unit unit;
    private Vector3 offset;

    public Allegiance Allegiance { get; private set; }
    public float Attack => unit.UnitSheetData.UnitLevelData[unit.Level].attackInField;
    public float HP => unit.UnitSheetData.UnitLevelData[unit.Level].hp;

    public void Init(Unit unit, Allegiance allegiance, int level)
    {
        this.Allegiance = allegiance;
        this.unit = unit;
        this.Level = level;
    }

    public void SetOffset(Vector3 offset)
    {
        view.localPosition = offset;
    }

    public void Move(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void ActivateCollider()
    {
        collider.enabled = true;
    }

    private void Reset()
    {
        collider = GetComponent<SphereCollider>();
        view = GetComponentInChildren<Renderer>().transform;
    }
}
