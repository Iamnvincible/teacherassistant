using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TeacherAssistant.Converter
{
    class DaynameValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int d = (int)value;
            string day = "";
            switch (d)
            {
                case 0:day = "星期日"; break;
                case 1:day = "星期一"; break;
                case 2:day = "星期二"; break;
                case 3:day = "星期三"; break;
                case 4:day = "星期四"; break;
                case 5:day = "星期五"; break;
                case 6: day = "星期六"; break;
            }
            return day;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
