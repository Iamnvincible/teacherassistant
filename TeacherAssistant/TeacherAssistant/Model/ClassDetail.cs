using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.Model
{

    public enum week : byte
    {
        星期一,
        星期二,
        星期三,
        星期四,
        星期五,
        星期六,
        星期日,
    }
    public class ClassDetail : IComparer<ClassDetail>
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
            return this.id == other.id && this.StudentListUrl == other.StudentListUrl && this.CourseDay == other.CourseDay && this.CourseTime == other.CourseTime;
        }

        public int Compare(ClassDetail x, ClassDetail y)
        {
            int xd = strweektoint(x.CourseDay);
            int yd = strweektoint(y.CourseDay);
            int xt = strtimetoint(x.CourseTime);
            int yt = strtimetoint(y.CourseTime);
            if (xd != yd)
                return xd - yd;
            else
                return xt - yt;
        }
        private int strweektoint(string w)
        {
            if (w == "星期一")
                return 1;
            if (w == "星期二")
                return 2;
            if (w == "星期三")
                return 3;
            if (w == "星期四")
                return 4;
            if (w == "星期五")
                return 5;
            if (w == "星期六")
                return 6;
            if (w == "星期日" || w == "星期天")
                return 7;
            else
                return 0;
        }
        private int strtimetoint(string t)
        {
            if (t == "一二节")
                return 1;
            if (t == "三四节")
                return 2;
            if (t == "五六节")
                return 3;
            if (t == "七八节")
                return 4;
            if (t == "九十节")
                return 5;
            if (t == "十一二节")
                return 6;
            else
                return 0;
        }
    }
}
