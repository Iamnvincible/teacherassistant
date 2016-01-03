using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.Model
{
    public class Homework
    {
        public string StuName { get; set; }
        public string StuNum { get; set; }
        public string ClassNum { get; set; }
        //作业分数
        public decimal Score { get; set; }
        public string CourseNum { get; set; }
        //作业次数
        public int Count { get; set; }
        public string Stulisturl { get; set; }

    }
}
