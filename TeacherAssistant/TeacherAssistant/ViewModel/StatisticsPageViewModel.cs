using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.Model;

namespace TeacherAssistant.ViewModel
{
    public class StatisticsPageViewModel
    {
        public List<ClassDetail> cd { get; set; }
        public StatisticsPageViewModel()
        {
            cd = App.classtable;
        }
    }
}
