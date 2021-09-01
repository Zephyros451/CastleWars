using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector3> StartPointSet;
    public event Action<Vector3> EndPointSet;
    public event Action InputStopped;

    private Tower firstTower;
    private Tower secondTower;
    private bool isDragged;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50, 1 << 0))
            {
                if(hit.collider.TryGetComponent(out Tower tower))
                {
                    if (tower.Allegiance != Allegiance.Player)
                    {
                        firstTower = null;
                        return;
                    }

                    StartPointSet?.Invoke((tower.transform.position));
                    isDragged = true;
                    firstTower = tower;
                }
            }
        }

        if(Input.GetMouseButton(0))
        {
            if(isDragged && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50, 1 << 0))
            {
                if(hit.collider.TryGetComponent(out Tower tower))
                {
                    EndPointSet?.Invoke(tower.transform.position);
                    return;
                }

                EndPointSet?.Invoke(hit.point);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50, 1 << 0))
            {
                if (hit.collider.TryGetComponent(out Tower tower) && firstTower != null)
                {
                    secondTower = tower;

                    if (firstTower == secondTower)
                    {
                        firstTower = null;
                        secondTower = null;

                        InputStopped?.Invoke();
                        isDragged = false;

                        return;
                    }
                    
                    firstTower.SendTroopTo(secondTower);
                }
            }

            InputStopped?.Invoke();
            isDragged = false;
        }
    }
}
