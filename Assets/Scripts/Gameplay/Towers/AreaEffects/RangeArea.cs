using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeArea : BaseAreaEffect
{
    [SerializeField] private Tower tower;
    [SerializeField] private SphereCollider rangeCollider;

    private List<Model> models = new List<Model>();
    private WaitForSeconds shootInterval = new WaitForSeconds(0.8f);

    public float RangeDistance => tower.TowerSheetData.TowerLevelData[tower.Mediator.Level].rangeDistance;
    public float RangeDamage => tower.TowerSheetData.TowerLevelData[tower.Mediator.Level].rangeDamage;

    private void Start()
    {
        StartCoroutine(Shoot());
        rangeCollider.radius = RangeDistance;
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            yield return shootInterval;

            for (int i = models.Count - 1; i >= 0; i--)
            {
                if(models[i] == null)
                {
                    models.Remove(models[i]);
                }
            }    

            if (models.Count != 0)
            {
                int random = Random.Range(0, models.Count);
                var model = models[random];
                model.ApplyDamage(RangeDamage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Model model))
        {
            if(model.Allegiance != tower.Allegiance)
                models.Add(model);
        }
    }

    private void Reset()
    {
        tower = transform.parent.GetComponent<Tower>();
    }
}
