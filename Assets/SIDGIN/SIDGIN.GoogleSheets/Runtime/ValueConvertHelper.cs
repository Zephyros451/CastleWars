using System.Collections.Generic;
namespace SIDGIN.GoogleSheets
{
    using Common;
    public static class ValueConvertHelper
    {
        static Dictionary<System.Type, IValueConverter> valueConverters;
        static ValueConvertHelper()
        {
            valueConverters = new Dictionary<System.Type, IValueConverter>();
            var types = InterfaceHelper.GetChildTypesByInterface<IValueConverter>();
            foreach (var type in types)
            {
                var converter = System.Activator.CreateInstance(type) as IValueConverter;
                if (converter != null && !valueConverters.ContainsKey(converter.Type))
                {
                    valueConverters.Add(converter.Type, converter);
                }
            }
        }

        public static object Convert(string input, System.Type type)
        {
            if (valueConverters == null)
                return null;
            System.Type converterType = type;
            if (converterType.IsEnum)
            {
                converterType = typeof(System.Enum);
            }
            IValueConverter converter;
            if (valueConverters.TryGetValue(converterType, out converter))
            {
                return converter.Convert(input, type);
            }
            else
            {
                return null;
            }
        }
        public static string Convert(object input)
        {
            if (valueConverters == null)
                return null;
            if (input == null)
                return "";
            System.Type converterType = input.GetType();
            if (converterType.IsEnum)
            {
                converterType = typeof(System.Enum);
            }
            IValueConverter converter;
            if (valueConverters.TryGetValue(converterType, out converter))
            {
                return converter.Convert(input);
            }
            else
            {
                return null;
            }
        }
    }
}