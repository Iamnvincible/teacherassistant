using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TeacherAssistant.Model;

namespace TeacherAssistant.Converter
{
    class ListCountValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int[] weeks = (int[])value;
            if(weeks.Length==1)
            {
                return weeks[0] + " 周";
                
            }
            else if (weeks[weeks.Length-1] - weeks[0] == weeks.Length - 1)
            {
                return $"{weeks[0]}-{weeks[weeks.Length-1]} 周";
            }
            else if(weeks.All(x=>x%2==0))
            {
                return $"{weeks[0]}-{weeks[weeks.Length - 1]} 双周";
            }
            else if (weeks.All(x=>x%2!=0))
            {
                return $"{weeks[0]}-{weeks[weeks.Length - 1]} 单周";
            }
            else
            {
                string result = "";
                foreach (var item in weeks)
                {
                    result += item.ToString() + " ";
                }
                return result + " 周";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
