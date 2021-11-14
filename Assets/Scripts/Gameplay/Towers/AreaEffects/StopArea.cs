using UnityEngine;

public class StopArea : BaseAreaEffect
{
    [SerializeField] private Tower tower;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Model model))
        {
            if (model.Allegiance != tower.Allegiance)
            {
                model.RaiseEnteredAreaTrigger(this);
            }
        }
    }

    private void Reset()
    {
        tower = transform.parent.GetComponent<Tower>();
    }
}
