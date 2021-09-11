using System;
using System.Collections.Generic;
using System.Linq;

namespace SIDGIN.GoogleSheets.Internal
{
    public class ListOfStringConverter : IValueConverter
    {
        public Type Type => typeof(List<string>);

        public object Convert(string input, Type type)
        {
            var values = input.Split(',');
            if (values != null && values.Length > 0)
            {
                return values.ToList();
            }
            return null;
        }

        public string Convert(object input)
        {
            if(input is IList<string>)
            {
                var list = (IList<string>)input;
                return list.Aggregate((x, y) => $"{x},{y}");
            }
            return "";
        }
    }
}