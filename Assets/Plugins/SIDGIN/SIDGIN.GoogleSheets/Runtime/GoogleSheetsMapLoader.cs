using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace SIDGIN.GoogleSheets
{
    using Common;
    public static class GoogleSheetsMapLoader
    {
        public static void Load(GoogleSheetsManager manager, ScriptableObject scriptableObject)
        {
            if (scriptableObject == null)
                throw new ArgumentNullException("scriptableObject");

            var type = scriptableObject.GetType();
            var googleAttribute = (GoogleSheetAttribute)Attribute.GetCustomAttribute(type, typeof(GoogleSheetAttribute));
            if (googleAttribute != null)
            {
                var collectionSetType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollectionSet<>));
                if (collectionSetType == null)
                {
                    return;
                }
                var elementType = collectionSetType.GetGenericArguments().Single();
                var setCollectionMethod = collectionSetType.GetMethod("SetCollection",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (setCollectionMethod == null)
                    throw new InvalidOperationException("SG Google Sheets Api. SetCollection method not found!");
                var data = manager.LoadData(googleAttribute.tableName, new GoogleSheetsMapConverter(), elementType, googleAttribute.sheetName);
                setCollectionMethod.Invoke(scriptableObject, new object[] { ConvertList(data, elementType) });
            }

        }
        public static void Load(GoogleSheetsManager manager, ISheetSaver saver = null)
        {
            var loadTypes = InterfaceHelper.GetChildTypesByInterface<ICollectionSet>().ToList();
            foreach (var type in loadTypes)
            {
                var resources = Resources.FindObjectsOfTypeAll(type);
                foreach (var resource in resources)
                {
                    var scriptableObject = resource as ScriptableObject;
                    if (scriptableObject != null)
                    {
                        Load(manager, scriptableObject);
                        if (saver != null)
                            saver.Save(scriptableObject);
                    }
                }
            }

        }
        public static object ConvertList(IList<object> source, Type elementType)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);
            if (source != null)
            {
                foreach (var value in source)
                {
                    list.Add(value);
                }
                return list;
            }
            return null;
        }
    }
}