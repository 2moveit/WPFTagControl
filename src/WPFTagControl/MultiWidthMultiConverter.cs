using System;
using System.Windows.Data;

namespace WPFTagControl
{
    /// <remarks>http://stackoverflow.com/questions/4858449/creating-complex-calculations-for-grid-column-width-in-wpf</remarks>
    public class MaxWidthMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double controlStackPanelWidth = (double)values[0];
            double iconWidth = (double)values[1];
            double createTagButtonWidth = (double)values[2];
            return controlStackPanelWidth - (iconWidth + createTagButtonWidth);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}