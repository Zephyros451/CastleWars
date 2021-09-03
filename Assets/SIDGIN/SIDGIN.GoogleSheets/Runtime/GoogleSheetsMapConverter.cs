using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace SIDGIN.GoogleSheets
{
    internal class GoogleSheetsMapConverter : IRowDataConverter
    {
        IList<string> headers;
        public GoogleSheetsMapConverter(IList<string> headers = null)
        {
            this.headers = headers;
        }
        class TypeInfo
        {
            public Type type;
            public List<KeyValuePair<FieldInfo, GoogleSheetParamAttribute>> fields = new List<KeyValuePair<FieldInfo, GoogleSheetParamAttribute>>();
            public List<FieldInfo> dataFields = new List<FieldInfo>();
        }
        List<TypeInfo> typeInfos = new List<TypeInfo>();
        public IList<object> Convert(IList<IList<object>> table, Type outObjectType)
        {
            var result = new List<object>();
            if (headers == null)
                headers = table[0].Cast<string>().ToList();

            for (int i = 1; i < table.Count; i++)
            {
                var item = GetParsedObject(table[i], outObjectType);
                if (item != null)
                    result.Add(item);

            }
            return result;
        }

        object GetParsedObject(IList<object> values, Type outputType)
        {
            var typeInfo = GetTypeInfo(outputType);
            var item = Activator.CreateInstance(outputType);
            bool isSetup = false;

            if (typeInfo.fields != null && typeInfo.fields.Count > 0)
            {

                foreach (var field in typeInfo.fields)
                {
                    if (field.Value.valueIndex < values.Count)
                    {
                        var value = ValueConvertHelper.Convert(values[field.Value.valueIndex].ToString(), field.Key.FieldType);
                        if (value != null)
                        {
                            field.Key.SetValue(item, value);
                            isSetup = true;
                        }
                    }
                }

            }

            if (typeInfo.dataFields != null && typeInfo.dataFields.Count > 0)
            {
                foreach (var field in typeInfo.dataFields)
                {
                    var dataObj = GetParsedObject(values, field.FieldType);
                    if (dataObj != null)
                    {
                        field.SetValue(item, dataObj);
                        isSetup = true;
                    }
                }
            }
            if (isSetup)
                return item;
            return null;
        }
        TypeInfo GetTypeInfo(Type type)
        {
            var typeInfo = typeInfos.FirstOrDefault(x => x.type == type);
            if (typeInfo != null)
                return typeInfo;
            typeInfo = new TypeInfo { type = type };

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.IsDefined(typeof(GoogleSheetParamAttribute), false));
            foreach (var field in fields)
            {
                var attribute = (GoogleSheetParamAttribute)Attribute.GetCustomAttribute(field, typeof(GoogleSheetParamAttribute));
                if (!attribute.isCustomType && headers.Contains(attribute.columnName))
                {
                    attribute.valueIndex = headers.IndexOf(attribute.columnName);
                    typeInfo.fields.Add(new KeyValuePair<FieldInfo, GoogleSheetParamAttribute>(field, attribute));
                }
                else if (attribute.isCustomType && field.FieldType.IsClass)
                {
                    typeInfo.dataFields.Add(field);
                }
            }
            return typeInfo;
        }


        void UpdateTableLine(IList<object> values, object inputObject)
        {
            var typeInfo = GetTypeInfo(inputObject.GetType());
            if (typeInfo.fields != null && typeInfo.fields.Count > 0)
            {

                foreach (var field in typeInfo.fields)
                {
                    if (field.Value.valueIndex < values.Count)
                    {
                        values[field.Value.valueIndex] = ValueConvertHelper.Convert(field.Key.GetValue(inputObject));
                    }
                }

            }

            if (typeInfo.dataFields != null && typeInfo.dataFields.Count > 0)
            {
                foreach (var field in typeInfo.dataFields)
                {
                    var childObject = field.GetValue(inputObject);
                    if (childObject != null)
                    {
                        UpdateTableLine(values, childObject);
                    }
                }
            }
        }

        public IList<IList<object>> Convert(IList<object> data)
        {
            if (headers == null)
                return new List<IList<object>>();

            var result = new List<IList<object>>();
            result.Add(headers.Cast<object>().ToList());
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == null)
                    continue;
                object[] values = new object[headers.Count];
                UpdateTableLine(values, data[i]);
                result.Add(values);

            }
            return result;
        }
    }

}