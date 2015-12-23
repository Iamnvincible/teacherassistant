using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TeacherAssistant.Converter
{
    public class SexImageValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sex = (string)value;
            if (sex.Contains("男"))
            {
                return "/Image/Boy-100.png";
            }
            else if(sex.Contains("女"))
            {
                return "/Image/Girl-100.png";
            }
            else
            {
                return "/image/Genderqueer-100.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
