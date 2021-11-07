using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Model : MonoBehaviour, IModel
{
    [SerializeField, HideInInspector] private new SphereCollider collider;
    [SerializeField, HideInInspector] private Transform view;

    public int CurrentSegment { get; private set; }
    public int Level { get; private set; }
    public Allegiance Allegiance { get; private set; }
    public float Attack => UnitData.FieldAttack;
    public float HP => UnitData.HP;
    public UnitData UnitData { get; private set; }

    public void Init(UnitData unitData, Allegiance allegiance, int level)
    {
        this.UnitData = unitData;
        this.Allegiance = allegiance;
        this.Level = level;
    }

    public void IncrementSegment()
    {
        CurrentSegment++;
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
