using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeacherAssistant.DataBase;
using TeacherAssistant.Model;
using TeacherAssistant.ViewModel;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for IndexPage.xaml
    /// </summary>
    public partial class IndexPage : Page
    {
        IndexPageViewModel ipvm;
        public IndexPage()
        {
            InitializeComponent();
            getclasstable();
            ipvm = new IndexPageViewModel(); 
            this.DataContext = ipvm;
        }
        async void getclasstable()
        {
            List<ClassDetail> classtable = new List<ClassDetail>();
            string sql = "select * from classtable";
            await Task.Run(() =>
            {
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
                }
            });
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
        }

    }
}
