using System;
using System.Collections;
using System.Collections.Generic;
namespace SIDGIN.GoogleSheets
{
    public static class GoogleSheetsMapSender
    {
        public static void Send(GoogleSheetsManager manager, IList<object> sourceObjects)
        {
            if (sourceObjects == null)
                throw new ArgumentNullException("sourceObject");
            if (sourceObjects.Count == 0)
                return;
            if (sourceObjects[0] == null)
                return;
            var elementType = sourceObjects[0].GetType();
            var googleAttribute = (GoogleSheetAttribute)Attribute.GetCustomAttribute(elementType, typeof(GoogleSheetAttribute));
            if (googleAttribute != null)
            {
                var headers = manager.LoadHeaders(googleAttribute.tableName, googleAttribute.sheetName);
                manager.SendData(googleAttribute.tableName, new GoogleSheetsMapConverter(headers), sourceObjects);
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