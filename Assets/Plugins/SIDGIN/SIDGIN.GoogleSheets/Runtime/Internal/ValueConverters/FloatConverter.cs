using System;
using System.Globalization;

namespace SIDGIN.GoogleSheets.Internal
{
    public class FloatConverter : IValueConverter
    {
        public Type Type => typeof(float);

        public object Convert(string input, Type type)
        {
            float result = 0;
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            float.TryParse(input, NumberStyles.Any, ci, out result);
            return result;
        }

        public string Convert(object input)
        {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            return ((float)input).ToString(ci);
        }
    }
}