using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData unitData;
    [SerializeField] private UnitSheetData unitSheetData;

    private int level;
    private BezierCurve curve;
    private List<Vector3> path;
    private List<Model> models;
    private WaitForSeconds modelTimeSpacing;

    public UnitData UnitData => unitData;
    public UnitSheetData UnitSheetData => unitSheetData;
    public int Level => level;

    public void Init(BezierCurve curve, List<Model> models, Tower destination, int level)
    {
        this.curve = curve;
        path = curve.GetSegmentPoints();
        this.models = models;
        this.level = level;
        modelTimeSpacing = new WaitForSeconds(0.5f/unitSheetData.UnitLevelData[level].speed);

        StartCoroutine(ActivateModels());
    }

    private void OnEnable()
    {
        GameState.instance.YouLose += Stop;
        GameState.instance.YouWin += Stop;
    }

    private void OnDisable()
    {
        GameState.instance.YouLose -= Stop;
        GameState.instance.YouWin -= Stop;
    }

    private void Update()
    {
        if (curve != null)
        {
            for (int i = models.Count - 1; i >= 0; i--)
            {
                if(models[i] == null)
                {
                    models.Remove(models[i]);
                }
            }
            if(models.Count == 0)
            {
                Destroy(this.gameObject);
            }

            for (int i = 0; i < models.Count; i++)
            {
                if ((models[i].SegmentsTravelled < path.Count) && models[i].IsActive)
                {
                    models[i].transform.position = Vector3.MoveTowards(models[i].transform.position, path[models[i].SegmentsTravelled], unitSheetData.UnitLevelData[level].speed * Time.deltaTime);

                    var difference = Vector3.Distance(models[i].transform.position, path[models[i].SegmentsTravelled]);
                    if (difference < 0.1f)
                    {
                        models[i].SegmentsTravelled++;

                        if (models[i].SegmentsTravelled > path.Count / 4)
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

    private void Stop()
    {
        for (int i = 0; i < models.Count; i++)
        {
            models[i].IsActive = false;
        }
    }
}
