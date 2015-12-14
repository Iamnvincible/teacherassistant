using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TeacherAssistant.Model;

namespace TeacherAssistant
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static List<ClassDetail> classtable = null;
        public static string Databasefilepath = null;
        public static int WeekCount = 0;
        //public static DateTime Now;
    }
}
