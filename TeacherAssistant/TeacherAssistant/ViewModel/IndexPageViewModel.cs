using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.DataBase;
using TeacherAssistant.Helper;
using TeacherAssistant.Model;

namespace TeacherAssistant.ViewModel
{
    public class IndexPageViewModel
    {
        public DateTime Now { get; set; }
        public string Date { get; set; }
        public DayOfWeek Week { get; set; }
        public ClassDetail currentcourse { get; private set; }
        public ClassDetail nextcourse { get; private set; }
        public List<ClassDetail> listclass { get; set; }
        //private List<ClassDetail> listclass { get; set; }
        public IndexPageViewModel()
        {
            setdate();
            getclasstable();
            setcourse();
            // Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // cfa.AppSettings.Settings["week"].Value = "kkk";
            //cfa.Save();
            //string sss = ConfigurationManager.AppSettings["week"];
            //ConfigurationSettings.AppSettings["week"];
            //Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //cfa.AppSettings.Settings.Add()
        }
        //得到当前时间
        void setdate()
        {
            Now = DateTime.Now.ToLocalTime();
            Date = Now.ToShortDateString();
            Week = Now.DayOfWeek;
        }
        //从数据库获取课表
        void getclasstable()
        {
            List<ClassDetail> classtable = new List<ClassDetail>();
            string sql = "select * from classtable";

            OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            while (reader.Read())
            {
                ClassDetail tcsa = new ClassDetail();
                tcsa.CourseNum = reader["coursenum"].ToString();
                tcsa.CourseName = reader["coursename"].ToString();
                tcsa.Classroom = reader["classroom"].ToString();
                tcsa.LastWeeks = Array.ConvertAll<string, int>(reader["lastweeks"].ToString().Split(','), s => int.Parse(s));
                tcsa.ClassType = reader["classtype"].ToString();
                tcsa.Subject = reader["subject"].ToString();
                tcsa.StuClassNum = reader["stuclassnum"].ToString().Split(',');
                tcsa.CourseDay = reader["courseday"].ToString();
                tcsa.CourseTime = reader["coursetime"].ToString();
                tcsa.StudentListUrl = reader["stulisturl"].ToString();
                classtable.Add(tcsa);

                //比较是否是同一节课，只是周数不同
                for (int i = 0; i < classtable.Count - 1; i++)
                {
                    for (int j = i + 1; j < classtable.Count; j++)
                    {
                        if (classtable[i].Compare(classtable[j]))
                        {
                            int[] int1 = classtable[i].LastWeeks;
                            int[] int2 = classtable[j].LastWeeks;
                            int[] all = new int[int1.Length + int2.Length];
                            int1.CopyTo(all, 0);
                            int2.CopyTo(all, int1.Length);
                            Array.Sort(all);
                            classtable[i].LastWeeks = all;
                            classtable.Remove(classtable[j]);
                        }
                    }
                }
                for (int i = 0; i < classtable.Count; i++)
                {
                    if (classtable[i].LastWeeks[0] == -1)
                    {
                        var v = from n in classtable where n.StudentListUrl == classtable[i].StudentListUrl select n.LastWeeks;
                        var all = v.ToArray();
                        for (int k = 0; k < all.GetLength(0); k++)
                        {
                            if (all[k].Length > 1)
                            {
                                var odd = from n in all[k] where n % 2 != 0 select n;
                                classtable[i].LastWeeks = odd.ToArray();
                            }
                        }
                    }
                    if (classtable[i].LastWeeks[0] == -2)
                    {
                        var v = from n in classtable where n.StudentListUrl == classtable[i].StudentListUrl select n.LastWeeks;
                        var all = v.ToArray();
                        for (int k = 0; k < all.GetLength(0); k++)
                        {
                            if (all[k].Length > 1)
                            {
                                var even = from n in all[k] where n % 2 == 0 select n;
                                classtable[i].LastWeeks = even.ToArray();
                            }
                        }
                    }
                }
                classtable.Sort(new ClassDetail());
                App.classtable = classtable;
                listclass = classtable;
            }
            reader.Close();
            AccessDBHelper.CloseConnectDB();
        }
        void setcourse()
        {
            //string time = Now.ToShortTimeString();
            //var t = DateTime.Parse(time).TimeOfDay;
            //现在是第几周的周几的哪个时间段
            //查/第几周/的周几/的哪个时间段/有没有课
            //本周有的课
            List<ClassDetail> temp = new List<ClassDetail>();
            foreach (var item in listclass)
            {
                DateTime firstday = new DateTime(2015, 9, 7, 0, 0, 0);
                int week = (int)((Now - firstday).TotalDays / 7) + 1;
                if (item.LastWeeks.Contains(week))
                {
                    temp.Add(item);
                }
            }
            if (temp.Count > 0)
            {
                CourseHelper.settimespan();
                //按周找课表
                int week = (int)Now.DayOfWeek;
                var n = Now.TimeOfDay;
                week = (int)week == 0 ? 7 : week;
                for (int i = 0; i < temp.Count; i++)
                {
                    int courseday = CourseHelper.transzhweektous(temp[i].CourseDay);
                    var aaaa = CourseHelper.transzhtimetoint(temp[i].CourseTime) - 1;
                    TimeSpan coursetime0 = CourseHelper.timespan[CourseHelper.transzhtimetoint(temp[i].CourseTime) - 1];
                    TimeSpan coursetime1 = CourseHelper.timespan[CourseHelper.transzhtimetoint(temp[i].CourseTime)];

                    if (week < courseday)
                    {
                        nextcourse = temp[i];
                        App.nextcourse = nextcourse;
                        break;
                    }
                    else if (week == courseday && n >= coursetime0 && n <= coursetime1)
                    {
                        currentcourse = temp[i];
                        if (i + 1 < temp.Count)
                        {
                            nextcourse = temp[i + 1];
                            App.nextcourse = nextcourse;
                        }
                        App.currentcourse = currentcourse;
                        
                        break;
                    }
                    else if (week == CourseHelper.transzhweektous(temp[i].CourseDay) && n < CourseHelper.timespan[CourseHelper.transzhtimetoint(temp[i].CourseTime) - 1])
                    {
                        nextcourse = temp[i];
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            //listclass
        }


        /* int transzhweektous(string zhweek)
          {
              int r = 0;
              switch (zhweek)
              {
                  case "星期一": r = 1; break;
                  case "星期二": r = 2; break;
                  case "星期三": r = 3; break;
                  case "星期四": r = 4; break;
                  case "星期五": r = 5; break;
                  case "星期六": r = 6; break;
                  case "星期日": r = 7; break;
              }
              return r;
          }
          //int transzhtimetoint(string zhtime)
          {
              int r = 0;
              switch (zhtime)
              {
                  case "一二节": r = 1; break;
                  case "三四节": r = 3; break;
                  case "五六节": r = 5; break;
                  case "七八节": r = 7; break;
                  case "九十节": r = 9; break;
                  default: r = 11; break;
              }
              return r;
          }
          */
    }
}
