using System;
using System.Collections;
using System.Collections.Generic;
namespace SIDGIN.GoogleSheets.Internal
{
    internal class GoogleSheetsLoadByMap : IGoogleDataLoader
    {
        public void Load(GoogleSheetsManager manager, ISheetSaver saver = null)
        {
            GoogleSheetsMapLoader.Load(manager, saver);
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