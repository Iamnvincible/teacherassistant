using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.DataBase;
using TeacherAssistant.Model;

namespace TeacherAssistant.ViewModel
{
    public class StatisticsPageViewModel
    {
        public List<ClassDetail> cd { get; set; }
        public List<int> countstus { get; set; } = new List<int>();
        public StatisticsPageViewModel()
        {
            cd = App.classtable;
            //cd = (from n in ori group n by n.StudentListUrl into g select g.First()).ToList() ;
        }
    }
}
