using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private List<Vector3> path;
    private Stack<Model> models = new Stack<Model>();
    private List<Model> activeModels = new List<Model>();
    private WaitForSeconds modelTimeSpacing;

    private float speedMultiplier = 1;

    public int Level { get; private set; }
    public BezierCurve Curve { get; private set; }

    private List<BaseAreaEffect> processedAreaEffects = new List<BaseAreaEffect>(1);

    public void Init(BezierCurve curve, DirectionType direction)
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

    public void AddModels(Stack<Model> newModels)
    {
        if (newModels.Count == 0)
            return;

        for (; newModels.Count > 0;)
        {
            models.Push(newModels.Pop());
        }
        modelTimeSpacing = new WaitForSeconds(0.55f / models.Peek().UnitData.Speed);
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

            Vector3 newPosition = Vector3.MoveTowards(activeModels[i].transform.position,
                path[activeModels[i].CurrentSegment],
                activeModels[i].UnitData.Speed * this.speedMultiplier * Time.deltaTime);
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

    private void ProcessAreaEffect(BaseAreaEffect areaEffect)
    {
        if(processedAreaEffects.Contains(areaEffect))
        {
            return;
        }
        processedAreaEffects.Add(areaEffect);

        if (areaEffect is StopArea)
        {
            var stopEffect = areaEffect as StopArea;
            DOTween.Sequence()
                .AppendCallback(() => speedMultiplier = 0)
                .AppendInterval(stopEffect.StopTime)
                .AppendCallback(() => speedMultiplier = 1);
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
                    model.EnteredAreaTrigger += ProcessAreaEffect;
                    model.SetOffset(Vector3.right * 0.5f);
                    activeModels.Add(model);

                    model = models.Pop();
                    model.EnteredAreaTrigger += ProcessAreaEffect;
                    model.SetOffset(Vector3.left * 0.5f);
                    activeModels.Add(model);

                    yield return modelTimeSpacing;
                }
                else
                {
                    var model = models.Pop();
                    model.EnteredAreaTrigger += ProcessAreaEffect;
                    activeModels.Add(model);
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
