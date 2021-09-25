using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData unitData;
    [SerializeField] private UnitSheetData unitSheetData;

    private List<Vector3> path;
    private Queue<Model> models = new Queue<Model>();
    private List<Model> activeModels = new List<Model>();
    private WaitForSeconds modelTimeSpacing;

    public UnitData UnitData => unitData;
    public UnitSheetData UnitSheetData => unitSheetData;
    public int Level { get; private set; }
    public BezierCurve Curve { get; private set; }

    public void Init(BezierCurve curve, Tower destination, int level, DirectionType direction)
    {
        Curve = curve;
        path = curve.GetSegmentPoints();
        Level = level;
        modelTimeSpacing = new WaitForSeconds(0.55f/unitSheetData.UnitLevelData[level].speed);

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

    public void AddModels(List<Model> newModels)
    {
        for (int i = 0; i < newModels.Count; i++)
        {
            models.Enqueue(newModels[i]);
        }
    }

    private void Update()
    {
        CleanUpActiveModelsList();
        MoveModels();
    }

    private void CleanUpActiveModelsList()
    {
        if (activeModels.Count > 0)
        {
            for (int i = activeModels.Count - 1; i >= 0; i--)
            {
                if (activeModels[i] == null)
                {
                    activeModels.Remove(activeModels[i]);
                }
            }
        }
    }

    private void MoveModels()
    {
        for (int i = 0; i < activeModels.Count; i++)
        {
            if (activeModels[i].SegmentsTravelled == path.Count)
                continue;

            activeModels[i].transform.position = Vector3.MoveTowards(activeModels[i].transform.position, path[activeModels[i].SegmentsTravelled], unitSheetData.UnitLevelData[Level].speed * Time.deltaTime);

            var difference = Vector3.Distance(activeModels[i].transform.position, path[activeModels[i].SegmentsTravelled]);
            if (difference < 0.1f)
            {
                activeModels[i].SegmentsTravelled++;

                if (activeModels[i].SegmentsTravelled > path.Count / 2)
                {
                    activeModels[i].ActivateCollider();
                }
            }
        }
    }

    private IEnumerator ActivateModels()
    {
        while (true)
        {
            if (models.Count > 0)
            {
                activeModels.Add(models.Dequeue());
                yield return modelTimeSpacing;
            }
            else
            {
                yield return null;
            }
        }
    }
}
