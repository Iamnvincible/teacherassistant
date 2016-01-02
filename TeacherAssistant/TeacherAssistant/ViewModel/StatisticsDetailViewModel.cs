using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.Model;

namespace TeacherAssistant.ViewModel
{
    public class StatisticsDetailViewModel
    {
        private ClassDetail detailcourse;
        public ClassDetail DetailCourse
        {
            set
            {
                this.detailcourse = value;
            }
            get { return detailcourse; }
        }

        public StatisticsDetailViewModel()
        {

        }
    }
}
