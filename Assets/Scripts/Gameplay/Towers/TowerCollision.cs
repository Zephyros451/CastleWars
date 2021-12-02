using System;
using UnityEngine;

public class TowerCollision : MonoBehaviour
{
    public event Action<UnitData> AllyCame;
    public event Action<Model> TowerAttacked;

    [SerializeField, HideInInspector] private Tower tower;
    public Tower Tower => tower;    

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Model model))
        {
            OnTowerEntered(model);
        }
    }

    private void OnTowerEntered(Model model)
    {
        if (model.Allegiance == tower.Allegiance)
        {
            AllyCame?.Invoke(model.UnitData);
        }
        else
        {
            TowerAttacked?.Invoke(model);
        }

        Destroy(model.gameObject);
    }

    private void Reset()
    {
        tower = transform.parent.GetComponent<Tower>();
    }
}
