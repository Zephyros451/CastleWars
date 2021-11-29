using UnityEngine;

public class StopArea : BaseAreaEffect
{
    [SerializeField] private Tower tower;

    public float StopTime => tower.TowerSheetData.TowerLevelData[tower.Mediator.Level].aoeTime;

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
