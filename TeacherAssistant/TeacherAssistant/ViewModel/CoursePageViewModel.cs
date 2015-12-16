using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.Model;

namespace TeacherAssistant.ViewModel
{
    public class CoursePageViewModel
    {
        public ClassDetail currentcourse { get; set; }
        public ClassDetail nextcourse { get; set; }
        public List<string> coursename;

        public CoursePageViewModel()
        {
            currentcourse = App.currentcourse;
            nextcourse = App.nextcourse;
            coursename = (from n in App.classtable.Distinct().ToList() where n.CourseNum != null select n.CourseName).ToList();
        }
    }
}
