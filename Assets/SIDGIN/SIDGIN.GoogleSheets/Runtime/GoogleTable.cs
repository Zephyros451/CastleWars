using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIDGIN.Common;
public class GoogleTable<TItem> : ScriptableObject, ICollectionSet<TItem> where TItem : class
{
    [SerializeField]
    List<TItem> items;

    public List<TItem> Rows { get { return items; } }
    void ICollectionSet<TItem>.SetCollection(List<TItem> data)
    {
        items = data;
    }
}
