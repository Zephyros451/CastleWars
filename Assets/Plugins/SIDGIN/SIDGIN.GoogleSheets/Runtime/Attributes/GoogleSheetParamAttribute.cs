using System;
namespace SIDGIN.GoogleSheets
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GoogleSheetParamAttribute : Attribute
    {
        public bool isCustomType;
        public string columnName;
        public int valueIndex;
        public GoogleSheetParamAttribute(string columnName = "")
        {
            if (string.IsNullOrEmpty(columnName))
            {
                isCustomType = true;
                return;
            }
            this.columnName = columnName;
        }
    }
}