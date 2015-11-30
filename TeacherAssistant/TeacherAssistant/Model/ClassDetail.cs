using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.Model
{
    public class ClassDetail
    {
        /// <summary>
        /// 课表中的id，顺序
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 课程编号
        /// </summary>
        public string CourseNum { get; set; }
        /// <summary>
        /// 课程名
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 上课教室
        /// </summary>
        public string Classroom { get; set; }
        /// <summary>
        /// 课程持续周
        /// </summary>
        public int[] LastWeeks { get; set; }
        /// <summary>
        /// 课程类型
        /// </summary>
        public string ClassType { get; set; }
        /// <summary>
        /// 上课班级专业
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 上课班级
        /// </summary>
        public string[] StuClassNum { get; set; }
        /// <summary>
        /// 教学班号
        /// </summary>
        public string StudentListUrl { get; set; }
        /// <summary>
        /// 上课星期
        /// </summary>
        public string CourseDay { get; set; }
        /// <summary>
        /// 上课时段
        /// </summary>
        public string CourseTime { get; set; }

        public bool Compare(ClassDetail other)
        {
            return this.id == other.id && this.StudentListUrl == other.StudentListUrl && this.CourseDay == other.CourseDay&&this.CourseTime==other.CourseTime;
        
        

        }
    }
}
