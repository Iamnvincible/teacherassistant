using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using TeacherAssistant.DataBase;
using MahApps.Metro.Controls;
using TeacherAssistant.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for ClassListWindow.xaml
    /// </summary>
    public partial class ClassListWindow
    {
        List<ClassDetail> classtable = new List<ClassDetail>();
        List<TeachClassStu> studatalist = new List<TeachClassStu>();
        public ClassListWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //getdata();
            getclasstable();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Loaddata.IsActive = true;
            List<TeachClassStu> studatalist = new List<TeachClassStu>();
            //AccessDBHelper.ConnectDB(App.Databasefilepath);
            string sql = "select * from A041518124736";
            await Task.Run(() =>
            {
                OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                while (reader.Read())
                {
                    TeachClassStu tcsa = new TeachClassStu();
                    tcsa.StuNum = reader["stunum"].ToString();
                    tcsa.StuName = reader["stuname"].ToString();
                    tcsa.Sex = reader["sex"].ToString();
                    tcsa.Subject = reader["subject"].ToString();
                    tcsa.ClassNum = reader["classnum"].ToString();
                    tcsa.ClassState = reader["classstate"].ToString();
                    tcsa.ClassType = reader["classtype"].ToString();
                    tcsa.Num = Convert.ToInt32(reader["num"].ToString());
                    studatalist.Add(tcsa);
                }
            });
            await Task.Delay(1000);
            studatalist.Sort(new TeachClassStu());
            this.stulist.DataContext = studatalist;
            this.Loaddata.IsActive = false;
            //DataTable dt = new DataTable();
            //DataRow dr;
            //for (int i = 0; i < reader.FieldCount; i++)
            //{
            //    DataColumn dc;
            //    dc = new DataColumn(reader.GetName(i));
            //    dt.Columns.Add(dc);

            //}
            //while (reader.Read())
            //{
            //    dr = dt.NewRow();
            //    for (int i = 0; i < reader.FieldCount; i++)
            //    {
            //        dr[reader.GetName(i)] = reader[reader.GetName(i)].ToString();
            //     }
            //}

        }
        void getdata()
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + App.Databasefilepath);
            OleDbDataAdapter da = new OleDbDataAdapter("Select * from A041518124736", conn);
            DataSet ds = new DataSet();
            DataSet quertDs = new DataSet();
            OleDbCommandBuilder cmd = new OleDbCommandBuilder(da);
            conn.Open();
            da = new OleDbDataAdapter("Select * from A041518124736", conn);
            //da.InsertCommand = cmd.GetInsertCommand();
            //da.UpdateCommand = cmd.GetUpdateCommand();
            //da.DeleteCommand = cmd.GetDeleteCommand();
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds);
            conn.Close();
            stulist.DataContext = ds.Tables[0];
        }
        async void getclasstable()
        {
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
            this.table.ItemsSource = classtable;
            //List<ClassDetail> cd2 = new List<ClassDetail>();
            //var group = classtable.GroupBy(p => p.CourseNum);
            //foreach (var item in group)
            //{
            //    ClassDetail c=new  ClassDetail
            //}

            //classtable.ForEach(c =>
            //{
            //    var group = classtable.Where(a => a.CourseDay == c.CourseDay && a.CourseTime == c.CourseDay && a.CourseNum == c.CourseNum);
            //    var weeks = group.Select(t => t.LastWeeks).ToArray();

            //});
        }

        private void table_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClassDetail selecteditem = table.SelectedItem as ClassDetail;
            if (selecteditem != null)
            {
                Debug.WriteLine(selecteditem.CourseName);
                getstulist(selecteditem.StudentListUrl);
            }
        }
        async void getstulist(string strurl)
        {
            this.Loaddata.IsActive = true;
            this.stulist.DataContext = null;
            studatalist.Clear();
            //List<TeachClassStu> studatalist = new List<TeachClassStu>();
            //AccessDBHelper.ConnectDB(App.Databasefilepath);
            string sql = String.Format("select * from {0}", strurl);
            await Task.Run(() =>
            {
                OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                while (reader.Read())
                {
                    TeachClassStu tcsa = new TeachClassStu();
                    tcsa.StuNum = reader["stunum"].ToString();
                    tcsa.StuName = reader["stuname"].ToString();
                    tcsa.Sex = reader["sex"].ToString();
                    tcsa.Subject = reader["subject"].ToString();
                    tcsa.ClassNum = reader["classnum"].ToString();
                    tcsa.ClassState = reader["classstate"].ToString();
                    tcsa.ClassType = reader["classtype"].ToString();
                    tcsa.Num = Convert.ToInt32(reader["num"].ToString());
                    studatalist.Add(tcsa);
                }
            });
            await Task.Delay(1000);
            studatalist.Sort(new TeachClassStu());
            this.stulist.DataContext = studatalist;
            this.Loaddata.IsActive = false;
        }
    }
}
