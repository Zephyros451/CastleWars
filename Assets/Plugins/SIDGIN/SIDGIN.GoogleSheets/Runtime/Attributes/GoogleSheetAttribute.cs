using System;
namespace SIDGIN.GoogleSheets
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GoogleSheetAttribute : Attribute
    {
        public string tableName;
        public string sheetName;
        public GoogleSheetAttribute(string tableName, string sheetName)
        {
            this.tableName = tableName;
            this.sheetName = sheetName;
        }
    }
}
