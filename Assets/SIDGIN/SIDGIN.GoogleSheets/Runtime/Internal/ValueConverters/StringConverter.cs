using System;
namespace SIDGIN.GoogleSheets.Internal
{
    public class StringConverter : IValueConverter
    {
        public Type Type => typeof(string);

        public object Convert(string input, Type type)
        {
            return input;
        }

        public string Convert(object input)
        {
            return input.ToString();
        }
    }
}