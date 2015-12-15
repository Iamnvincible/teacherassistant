using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.DataBase;
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
        private List<ClassDetail> listclass { get; set; }
        List<TimeSpan> timespan { get; set; }
        public IndexPageViewModel()
        {
            settimespan();
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
                //按周找课表
                int week = (int)Now.DayOfWeek;
                var n = Now.TimeOfDay;
                week = (int)week == 0 ? 7 : week;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (week < transzhweektous(temp[i].CourseDay))
                    {
                        nextcourse = temp[i];
                        break;
                    }
                    else if (week == transzhweektous(temp[i].CourseDay) && n > timespan[transzhtimetoint(temp[i].CourseTime) - 1] && n < timespan[transzhtimetoint(temp[i].CourseTime)])
                    {
                        currentcourse = temp[i];
                        if (i + 1 < temp.Count)
                        {
                            nextcourse = temp[i + 1];
                        }
                        break;
                    }
                    else
                    {
                        nextcourse = temp[i];
                        break;
                    }

                }

            }


            //listclass
        }
        void settimespan()
        {
            timespan = new List<TimeSpan>();
            TimeSpan tp1 = DateTime.Parse("8:00").TimeOfDay;
            TimeSpan tp2 = DateTime.Parse("9:40").TimeOfDay;
            TimeSpan tp3 = DateTime.Parse("10:05").TimeOfDay;
            TimeSpan tp4 = DateTime.Parse("11:45").TimeOfDay;
            TimeSpan tp5 = DateTime.Parse("14:00").TimeOfDay;
            TimeSpan tp6 = DateTime.Parse("15:40").TimeOfDay;
            TimeSpan tp7 = DateTime.Parse("16:05").TimeOfDay;
            TimeSpan tp8 = DateTime.Parse("17:45").TimeOfDay;
            TimeSpan tp9 = DateTime.Parse("19:00").TimeOfDay;
            TimeSpan tp10 = DateTime.Parse("20:40").TimeOfDay;
            TimeSpan tp11 = DateTime.Parse("20:50").TimeOfDay;
            TimeSpan tp12 = DateTime.Parse("22:20").TimeOfDay;
            timespan.Add(tp1);
            timespan.Add(tp2);
            timespan.Add(tp3);
            timespan.Add(tp4);
            timespan.Add(tp5);
            timespan.Add(tp6);
            timespan.Add(tp7);
            timespan.Add(tp8);
            timespan.Add(tp9);
            timespan.Add(tp10);
            timespan.Add(tp11);
            timespan.Add(tp12);
        }
        //8:00-9:40
        //10:05-11:45
        //14:00-15:40
        //16:05-17:45
        //19:00-20:40
        //20:50-10:20

        int transzhweektous(string zhweek)
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
        int transzhtimetoint(string zhtime)
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

    }
}
