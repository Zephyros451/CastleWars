using System;
using UnityEngine;

public class TowerCollision : MonoBehaviour
{
    [SerializeField, HideInInspector] private Tower tower;
    public Tower Tower => tower;

    public event Action<Model> TowerAttacked;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Model model))
        {
            TowerAttacked?.Invoke(model);
        }
    }

    private void Reset()
    {
        tower = transform.parent.GetComponent<Tower>();
    }
}
