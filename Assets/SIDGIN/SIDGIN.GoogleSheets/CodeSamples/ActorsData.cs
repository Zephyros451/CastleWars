using System.Collections.Generic;
using UnityEngine;
using SIDGIN.GoogleSheets;
using SIDGIN.Common;

[CreateAssetMenu(fileName = "ActorsData", menuName = "Test/ActorsData")]
[GoogleSheet("ActorsTable","Actors")]
public class ActorsData : ScriptableObject, ICollectionSet<ActorData>
{
    [SerializeField]
    List<ActorData> actors;

    public List<ActorData> Actors => actors;

    void ICollectionSet<ActorData>.SetCollection(List<ActorData> data)
    {
        actors = data;
    }
}

[System.Serializable, GoogleSheet("ActorsTable", "Actors")]
public class ActorData
{
    [SerializeField, GoogleSheetParam("id")]
    string id;
    [SerializeField, GoogleSheetParam("daily_income")]
    int dailyIncome;
    [SerializeField, GoogleSheetParam("test_bool")]
    bool testBool;
    [SerializeField, GoogleSheetParam("speed")]
    float speed;
    [SerializeField, GoogleSheetParam("weapons")]
    List<string> weapons;

    [SerializeField, GoogleSheetParam]
    ActorGroupData actorGroup;

}
[System.Serializable]
public class ActorGroupData
{
    [SerializeField, GoogleSheetParam("param1")]
    int param1;
    [SerializeField, GoogleSheetParam("param2")]
    int param2;
    [SerializeField, GoogleSheetParam("param3")]
    int param3;
}