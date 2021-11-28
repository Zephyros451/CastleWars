using UnityEngine;
using System;

[RequireComponent(typeof(SphereCollider))]
public class Model : MonoBehaviour, IModel
{
    [SerializeField] private new SphereCollider collider;
    [SerializeField] private MeshFilter view;
    [SerializeField] private Mesh[] levelSkins;

    public event Action<BaseAreaEffect> EnteredAreaTrigger;

    protected ITower target;

    public int CurrentSegment { get; private set; }
    public int Level { get; private set; }
    public Allegiance Allegiance { get; private set; }
    public float Attack => UnitData.FieldAttack;
    public float HP => UnitData.HP;
    public UnitData UnitData { get; private set; }

    public virtual void Init(UnitData unitData, Allegiance allegiance, int level, ITower target)
    {
        this.UnitData = unitData;
        this.Allegiance = allegiance;
        this.Level = level;
        this.target = target;

        this.view.mesh = levelSkins[level];
    }

    public void IncrementSegment()
    {
        CurrentSegment++;
    }

    public void SetOffset(Vector3 offset)
    {
        view.transform.localPosition = offset;
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

    public void RaiseEnteredAreaTrigger(BaseAreaEffect areaEffect)
    {
        EnteredAreaTrigger?.Invoke(areaEffect);
    }

    private void OnDestroy()
    {
        EnteredAreaTrigger = null;
    }

    private void Reset()
    {
        collider = GetComponent<SphereCollider>();
        view = GetComponentInChildren<MeshFilter>();
    }
}
