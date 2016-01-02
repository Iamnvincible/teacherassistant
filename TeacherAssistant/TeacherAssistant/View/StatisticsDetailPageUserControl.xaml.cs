using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
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
    /// Interaction logic for StatisticsDetailPageUserControl.xaml
    /// </summary>
    public partial class StatisticsDetailPageUserControl : UserControl
    {
        public StatisticsDetailPageUserControl()
        {
            InitializeComponent();
        }
        private void Goback(object sender, RoutedEventArgs e)
        {
            this.Content = new StatisticsPageUserControl();
        }

        private void homework(object sender, RoutedEventArgs e)
        {
            StatisticsDetailViewModel sd = (StatisticsDetailViewModel)this.DataContext;
            string sql = $"select * from Attendance,{sd.DetailCourse.StudentListUrl} where stulisturl='{sd.DetailCourse.StudentListUrl}' and Attendance.stunum={sd.DetailCourse.StudentListUrl}.stunum";
            OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            List<Arrive> attendancelist = new List<Arrive>();
            while (reader.Read())
            {
                Arrive a = new Arrive();
                a.StuNum = reader["Attendance.stunum"].ToString();
                a.StuName = reader["stuname"].ToString();
                a.CourseNum = reader["coursenum"].ToString();
                a.CourseTime = reader["coursetime"].ToString();
                a.ArriveState = Convert.ToInt32(reader["arrivestate"].ToString());
                attendancelist.Add(a);
            }
            MessageBox.Show(attendancelist[0].StuNum);
        }

        private void attendance(object sender, RoutedEventArgs e)
        {

        }

        private void edit(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
