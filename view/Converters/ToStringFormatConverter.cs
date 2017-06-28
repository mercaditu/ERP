using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Cognitivo.Converters
{
    public class ToStringFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 1)
                return System.Convert.ChangeType(values[0], targetType, culture);
            if (values.Length >= 2 && values[0] is IFormattable)
                return (values[0] as IFormattable).ToString((string)values[1], culture);
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var targetType = targetTypes[0];
            var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);
            if (nullableUnderlyingType != null)
            {
                if (value == null)
                    return new[] { (object)null };
                targetType = nullableUnderlyingType;
            }
            try
            {
                object parsedValue = ToStringFormatConverter.TryParse(value, targetType, culture);
                return parsedValue != DependencyProperty.UnsetValue
                    ? new[] { parsedValue }
                    : new[] { System.Convert.ChangeType(value, targetType, culture) };
            }
            catch
            {
                return null;
            }
        }

        // Some types have Parse methods that are more successful than their type converters at converting strings
        private static object TryParse(object value, Type targetType, CultureInfo culture)
        {
            object result = DependencyProperty.UnsetValue;
            string stringValue = value as string;

            if (stringValue != null)
            {
                try
                {
                    MethodInfo mi;
                    if (culture != null
                        && (mi = targetType.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static, null,
                            new[] { typeof(string), typeof(NumberStyles), typeof(IFormatProvider) }, null))
                        != null)
                    {
                        result = mi.Invoke(null, new object[] { stringValue, NumberStyles.Any, culture });
                    }
                    else if (culture != null
                        && (mi = targetType.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static, null,
                            new[] { typeof(string), typeof(IFormatProvider) }, null))
                        != null)
                    {
                        result = mi.Invoke(null, new object[] { stringValue, culture });
                    }
                    else if ((mi = targetType.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static, null,
                            new[] { typeof(string) }, null))
                        != null)
                    {
                        result = mi.Invoke(null, new object[] { stringValue });
                    }
                }
                catch (TargetInvocationException)
                {
                }
            }

            return result;
        }
    }
    
}
