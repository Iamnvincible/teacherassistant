using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.Model
{
    public class TeachClassStu:IComparer<TeachClassStu>
    {
        /// <summary>
        /// 学生学号
        /// </summary>
        public string StuNum { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StuName { get; set; }
        /// <summary>
        /// 学生性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 学生专业
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 学生班级
        /// </summary>
        public string ClassNum { get; set; }
        /// <summary>
        /// 选课状态
        /// </summary>
        public string ClassState { get; set; }
        /// <summary>
        /// 课程类型
        /// </summary>
        public string ClassType { get; set; }
        /// <summary>
        /// 顺序编号
        /// </summary>
        public int Num { get; set; }

        public int Compare(TeachClassStu x, TeachClassStu y)
        {
            return x.Num - y.Num;
        }
    }
}
