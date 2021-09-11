using SIDGIN.GoogleSheets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestDataSender : MonoBehaviour
{
    [SerializeField]
    ActorsData objectToSend;

    private void Start()
    {
        GoogleSheetsSendManager.Send(objectToSend.Actors.Cast<object>().ToList());
    }
}
