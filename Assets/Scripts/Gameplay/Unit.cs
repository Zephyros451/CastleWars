using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData unitData;
    [SerializeField] private UnitSheetData unitSheetData;

    private BezierCurve curve;
    private List<Vector3> path;
    private List<Model> models;
    private WaitForSeconds modelTimeSpacing;

    public UnitData UnitData => unitData;
    public UnitSheetData UnitSheetData => unitSheetData;
    public int Level { get; private set; }

    public void Init(BezierCurve curve, List<Model> models, Tower destination, int level, DirectionType direction)
    {
        this.curve = curve;
        path = curve.GetSegmentPoints();
        this.models = models;
        Level = level;
        modelTimeSpacing = new WaitForSeconds(0.5f/unitSheetData.UnitLevelData[level].speed);

        switch(direction)
        {
            case DirectionType.Backward:
                path.Reverse();
                break;
            case DirectionType.Forward:
                break;
            default:
                Debug.Log("Wrong direction type");
                break;
        }

        StartCoroutine(ActivateModels());
    }

    private void Update()
    {
        for (int i = models.Count - 1; i >= 0; i--)
        {
            if (models[i] == null)
            {
                models.Remove(models[i]);
            }
        }
        if (models.Count == 0)
        {
            Destroy(this.gameObject);
        }

        MoveModels();
    }

    private void MoveModels()
    {
        if (curve != null)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if ((models[i].SegmentsTravelled < path.Count) && models[i].IsActive)
                {
                    models[i].transform.position = Vector3.MoveTowards(models[i].transform.position, path[models[i].SegmentsTravelled], unitSheetData.UnitLevelData[Level].speed * Time.deltaTime);

                    var difference = Vector3.Distance(models[i].transform.position, path[models[i].SegmentsTravelled]);
                    if (difference < 0.1f)
                    {
                        models[i].SegmentsTravelled++;

                        if (models[i].SegmentsTravelled > path.Count / 2)
                        {
                            models[i].ActivateCollider();
                        }
                    }
                }
            }
        }
    }

    private IEnumerator ActivateModels()
    {
        for (int i = 0; i < models.Count; i++)
        {
            models[i].IsActive = true;
            yield return modelTimeSpacing;
        }
    }
}
