using System;
namespace SIDGIN.GoogleSheets.Internal
{
    public class BoolConverter : IValueConverter
    {
        public Type Type => typeof(bool);

        public object Convert(string input, Type type)
        {
            bool result = false;
            bool.TryParse(input, out result);
            return result;
        }

        public string Convert(object input)
        {
            return ((bool)input).ToString();
        }
    }
}