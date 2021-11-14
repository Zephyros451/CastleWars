using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeArea : BaseAreaEffect
{
    [SerializeField] private Tower tower;

    private List<Model> models = new List<Model>();
    private WaitForSeconds shootInterval = new WaitForSeconds(0.8f);

    private void Start()
    {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            yield return shootInterval;

            if (models.Count != 0)
            {
                int random = Random.Range(0, models.Count);
                var model = models[random];
                models.Remove(model);
                if(model!=null)
                Destroy(model.gameObject);
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
