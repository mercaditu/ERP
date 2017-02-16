using System;
using System.Windows.Data;

namespace cntrl.Converters
{
    public class LevelToMarginConverter : IValueConverter
    {
        public int LeftMargin { get; set; }
        public int OtherMargin { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else
            { return true; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}