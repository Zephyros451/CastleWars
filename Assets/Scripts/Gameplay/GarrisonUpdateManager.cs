using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarrisonUpdateManager : MonoBehaviour
{
    [SerializeField] private Tower[] towers;

    private float updateCooldown = 1f;
    private float updateTimer;

    private void Start()
    {
        updateTimer = updateCooldown;
    }

    private void Update()
    {
        updateTimer -= Time.deltaTime;
        if (updateTimer < 0f)
        {
            updateTimer = updateCooldown;
            foreach(var tower in towers)
            {
                tower.OnUpdateGarrison();
            }
        }
    }

    private void Reset()
    {
        towers = GetComponentsInChildren<Tower>();
    }
}
