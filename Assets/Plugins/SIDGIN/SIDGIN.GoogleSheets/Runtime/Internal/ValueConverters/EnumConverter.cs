using UnityEngine;
namespace SIDGIN.GoogleSheets.Internal
{
    public class EnumConverter : IValueConverter
    {
        public System.Type Type => typeof(System.Enum);

        public object Convert(string input, System.Type type)
        {
            object result = null;
            try
            {
                result = System.Enum.Parse(type, input);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error value convert while import from Google Sheets: " + ex);
            }
            return result;
        }

        public string Convert(object input)
        {
            var enm = (System.Enum)input;
            return enm.ToString();
        }
    }
}