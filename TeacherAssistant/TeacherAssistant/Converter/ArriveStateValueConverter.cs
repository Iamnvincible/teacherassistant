﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TeacherAssistant.Converter
{
    class ArriveStateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v = (int)value;
            switch (v)
            {
                case 0: return "没点";
                case 1:return "到~~";
                case 2:return "-没来-";
                case 3:return "请假";
                case 4:return "迟到";
                default:return "：-(";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
