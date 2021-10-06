using System;
using UnityEngine;

public class TowerCollision : MonoBehaviour
{
    public event Action AllyCame;
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
            AllyCame?.Invoke();
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
