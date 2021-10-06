using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData unitData;
    [SerializeField] private UnitSheetData unitSheetData;

    private List<Vector3> path;
    private Stack<Model> models = new Stack<Model>();
    private List<Model> activeModels = new List<Model>();
    private WaitForSeconds modelTimeSpacing;

    public UnitData UnitData => unitData;
    public UnitSheetData UnitSheetData => unitSheetData;
    public int Level { get; private set; }
    public BezierCurve Curve { get; private set; }

    public void Init(BezierCurve curve, Tower destination, DirectionType direction)
    {
        Curve = curve;
        path = curve.GetSegmentPoints();

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
            models.Push(newModels[i]);
        }
        modelTimeSpacing = new WaitForSeconds(0.55f / unitSheetData.UnitLevelData[newModels[0].Level].speed);
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
            if (activeModels[i].CurrentSegment == path.Count)
                continue;

            Vector3 newPosition = Vector3.MoveTowards(activeModels[i].transform.position, path[activeModels[i].CurrentSegment],
                                                      unitSheetData.UnitLevelData[activeModels[i].Level].speed * Time.deltaTime);
            Quaternion newRotation = Quaternion.identity;
            if (newPosition - activeModels[i].transform.position != Vector3.zero)
            {
                 newRotation = Quaternion.LookRotation(newPosition - activeModels[i].transform.position);
            }
            activeModels[i].Move(newPosition, newRotation);

            var difference = Vector3.Distance(activeModels[i].transform.position, path[activeModels[i].CurrentSegment]);
            if (difference < 0.1f)
            {
                activeModels[i].IncrementSegment();

                if (activeModels[i].CurrentSegment > path.Count / 2)
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
                if (models.Count % 2 == 0)
                {
                    var model = models.Pop();
                    model.SetOffset(-Vector3.left * 0.5f);
                    activeModels.Add(model);

                    model = models.Pop();
                    model.SetOffset(Vector3.left * 0.5f);
                    activeModels.Add(model);

                    yield return modelTimeSpacing;
                }
                else
                {
                    activeModels.Add(models.Pop());
                    yield return modelTimeSpacing;
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
