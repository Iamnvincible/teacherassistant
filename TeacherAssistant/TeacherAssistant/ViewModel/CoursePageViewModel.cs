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
            currentcourse = new ClassDetail();
            currentcourse = App.currentcourse??currentcourse;
            if (currentcourse.CourseName == null)
            {
                currentcourse.CourseName = "";
            }
            nextcourse = App.nextcourse;
            coursename = (from n in App.classtable.ToList() where n.CourseNum != null select n.CourseName).ToList();
            coursename = coursename.Distinct().ToList();
        }
    }
}
