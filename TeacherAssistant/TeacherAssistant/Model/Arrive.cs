using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.Model
{
    public class Arrive
    {
        public string StuNum { get; set; }
        public string  StuName { get; set; }
        public string CourseNum { get; set; }
        public string  CourseTime { get; set; }
        public int ArriveState { get; set; }
        public string Stulisturl { get; set; }
    }
}
