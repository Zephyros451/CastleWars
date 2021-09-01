using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingArrow : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private PlayerInput input;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private bool isEmpty = true;

    private void Start()
    {
        input.StartPointSet += SetStartPoint;
        input.EndPointSet += SetEndPoint;
        input.InputStopped += Clear;
    }

    private void Update()
    {
        Draw();
    }

    public void Draw()
    {
        if (isEmpty)
        {
            line.positionCount = 0;
            return;
        }

        Vector3[] points = new Vector3[180];
        line.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3(Mathf.Lerp(startPoint.x, endPoint.x, (i/180f)),
                                    Mathf.Sin((i/180f)+1),
                                    Mathf.Lerp(startPoint.z, endPoint.z, (i/180f)));
        }

        line.SetPositions(points);
    }

    private void SetStartPoint(Vector3 point)
    {
        startPoint = point;
        isEmpty = false;
    }

    private void SetEndPoint(Vector3 point)
    {
        endPoint = point;
    }

    private void Clear()
    {
        isEmpty = true;
        line.positionCount = 0;
        startPoint = Vector3.negativeInfinity;
        endPoint = Vector3.negativeInfinity;
    }

    private void Reset()
    {
        line = GetComponent<LineRenderer>();
        input = FindObjectOfType<PlayerInput>();
    }
}
