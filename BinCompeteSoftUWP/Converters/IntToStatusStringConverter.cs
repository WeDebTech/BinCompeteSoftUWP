using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BinCompeteSoftUWP.Converters
{
    public class IntToStatusStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int && (int)value == 0)
            {
                return "In progress";
            }
            else if (value is int && (int)value == 1)
            {
                return "In voting stage";
            }
            else
            {
                return "Ended";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
