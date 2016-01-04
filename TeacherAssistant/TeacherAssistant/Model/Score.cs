using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.Model
{
    class Score
    {
        public string StuName { get; set; }
        public string  StuNum { get; set; }
        public string StuListUrl { get; set; }
        public decimal Attendance { get; set; }
        public decimal Homework { get; set; }
        public decimal Addition { get; set; }
        public decimal Exam { get; set; }
        public decimal Final { get; set; }
    }
}
