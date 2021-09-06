using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIDGIN.GoogleSheets;
using SIDGIN.GoogleSheets.Internal;

public class GoogleSheetsUpdater : MonoBehaviour
{
    [SerializeField] private GoogleSheetsSettings settings;

    public void UpdateData()
    {
        GoogleSheetsLoadManager.Load(settings);
    }
}
