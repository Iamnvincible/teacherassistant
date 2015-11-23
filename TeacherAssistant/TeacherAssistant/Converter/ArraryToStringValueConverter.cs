using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TeacherAssistant.Converter
{
    class ArraryToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is String[])
            {
                string[] arr = value as string[];
                string re = "";
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i == 0)
                    {
                        re += arr[i];
                    }
                    else
                    re += ","+arr[i];
                }
                return re;
            }
            else if(value is int[])
            {
                int[] arr = value as int[];
                string re = "";
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i == 0)
                    {
                        re += arr[i];
                    }
                    else
                        re += "," + arr[i];
                }
                return re;

            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
