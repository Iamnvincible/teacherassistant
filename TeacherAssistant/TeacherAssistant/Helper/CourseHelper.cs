using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.Helper
{
    public static class CourseHelper
    {
        public static List<TimeSpan> timespan { get; set; }
        public static int transzhweektous(string zhweek)
        {
            int r = 0;
            switch (zhweek)
            {
                case "星期一": r = 1; break;
                case "星期二": r = 2; break;
                case "星期三": r = 3; break;
                case "星期四": r = 4; break;
                case "星期五": r = 5; break;
                case "星期六": r = 6; break;
                case "星期日": r = 7; break;
            }
            return r;
        }
        public static int transzhtimetoint(string zhtime)
        {
            int r = 0;
            switch (zhtime)
            {
                case "一二节": r = 1; break;
                case "三四节": r = 3; break;
                case "五六节": r = 5; break;
                case "七八节": r = 7; break;
                case "九十节": r = 9; break;
                default: r = 11; break;
            }
            return r;
        }
        public static void settimespan()
        {
            timespan = new List<TimeSpan>();
            TimeSpan tp1 = DateTime.Parse("8:00").TimeOfDay;
            TimeSpan tp2 = DateTime.Parse("9:40").TimeOfDay;
            TimeSpan tp3 = DateTime.Parse("10:05").TimeOfDay;
            TimeSpan tp4 = DateTime.Parse("11:45").TimeOfDay;
            TimeSpan tp5 = DateTime.Parse("14:00").TimeOfDay;
            TimeSpan tp6 = DateTime.Parse("15:40").TimeOfDay;
            TimeSpan tp7 = DateTime.Parse("16:05").TimeOfDay;
            TimeSpan tp8 = DateTime.Parse("17:45").TimeOfDay;
            TimeSpan tp9 = DateTime.Parse("19:00").TimeOfDay;
            TimeSpan tp10 = DateTime.Parse("20:40").TimeOfDay;
            TimeSpan tp11 = DateTime.Parse("20:50").TimeOfDay;
            TimeSpan tp12 = DateTime.Parse("22:20").TimeOfDay;
            timespan.Add(tp1);
            timespan.Add(tp2);
            timespan.Add(tp3);
            timespan.Add(tp4);
            timespan.Add(tp5);
            timespan.Add(tp6);
            timespan.Add(tp7);
            timespan.Add(tp8);
            timespan.Add(tp9);
            timespan.Add(tp10);
            timespan.Add(tp11);
            timespan.Add(tp12);
        }
        //8:00-9:40
        //10:05-11:45
        //14:00-15:40
        //16:05-17:45
        //19:00-20:40
        //20:50-10:20
    }
}
