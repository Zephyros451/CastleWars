using System;
namespace SIDGIN.GoogleSheets.Internal
{
    public class IntConverter : IValueConverter
    {
        public Type Type => typeof(int);

        public object Convert(string input, Type type)
        {
            int result = 0;
            int.TryParse(input, out result);
            return result;
        }

        public string Convert(object input)
        {
            return ((int)input).ToString();
        }
    }
}