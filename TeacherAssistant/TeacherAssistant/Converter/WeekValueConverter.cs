using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Configuration;

namespace TeacherAssistant.Converter
{
    public class WeekValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime firstday = new DateTime(2015, 9, 7,0,0,0);
            DateTime dt = (DateTime)value;
            int week = (int)((dt - firstday).TotalDays / 7) + 1;
            
            return "第"+week.ToString()+"周";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
