using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    private readonly float doubleClickTime = 0.3f;

    public event Action<Vector3> StartPointSet;
    public event Action<Vector3> EndPointSet;
    public event Action InputStopped;

    private Tower firstTower;
    private Tower secondTower;
    private bool isDragged;
    private float timeOfLastClick;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50, 1 << 0))
            {
                if(hit.collider.TryGetComponent(out TowerCollision collision))
                {
                    if (collision.Tower.Allegiance != Allegiance.Player)
                    {
                        firstTower = null;
                        return;
                    }

                    StartPointSet?.Invoke((collision.transform.position));
                    isDragged = true;
                    firstTower = collision.Tower;                    
                }
            }
        }

        if(Input.GetMouseButton(0))
        {
            if(isDragged && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50, 1 << 0))
            {
                if(hit.collider.TryGetComponent(out TowerCollision collision))
                {
                    EndPointSet?.Invoke(collision.transform.position);
                    return;
                }

                EndPointSet?.Invoke(hit.point);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            float timeSinceLastClick = Time.time - timeOfLastClick;

            if (timeSinceLastClick <= doubleClickTime)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50, 1 << 0))
                {
                    if(hit.collider.TryGetComponent(out TowerCollision collision))
                    {
                        if(collision.Tower.Allegiance == Allegiance.Player)
                        {
                            collision.Tower.Level.LevelUp();
                        }
                    }
                }
            }
            else
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50, 1 << 0))
                {
                    if (hit.collider.TryGetComponent(out TowerCollision collision) && firstTower != null)
                    {
                        secondTower = collision.Tower;

                        if (firstTower == secondTower)
                        {
                            firstTower = null;
                            secondTower = null;

                            InputStopped?.Invoke();
                            isDragged = false;

                            timeOfLastClick = Time.time;

                            return;
                        }

                        firstTower.TroopSender.SendTroopTo(secondTower);
                    }
                }
            }

            InputStopped?.Invoke();
            isDragged = false;
        }
    }
}
