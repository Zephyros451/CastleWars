﻿using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Model : MonoBehaviour, IModel
{
    [SerializeField, HideInInspector] private SphereCollider collider;
    [SerializeField, HideInInspector] private Transform view;    

    private Unit unit;
    private Vector3 offset;

    public int CurrentSegment { get; private set; }
    public int Level { get; private set; }
    public Allegiance Allegiance { get; private set; }
    public float Attack => unit.UnitSheetData.UnitLevelData[Level].attackInField;
    public float HP => unit.UnitSheetData.UnitLevelData[Level].hp;

    public void Init(Unit unit, Allegiance allegiance, int level)
    {
        this.Allegiance = allegiance;
        this.unit = unit;
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
