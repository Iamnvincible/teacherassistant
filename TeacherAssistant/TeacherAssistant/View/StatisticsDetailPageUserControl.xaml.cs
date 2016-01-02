using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<Arrive> attendancelist = new ObservableCollection<Arrive>();

        public StatisticsDetailPageUserControl()
        {
            InitializeComponent();
            this.tab.SelectionChanged += Tab_SelectionChanged;
        }

        private void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int si = this.tab.SelectedIndex;
            Debug.WriteLine("选中{0}", si);
            switch (si)
            {
                case 0: break;
                case 1:
                    break;
                case 2:
                    getattendance();
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }

        private void Goback(object sender, RoutedEventArgs e)
        {
            this.Content = new StatisticsPageUserControl();
        }

        private async void getattendance()
        {
            if (attendancelist != atten.ItemsSource)
            {

                StatisticsDetailViewModel sd = (StatisticsDetailViewModel)this.DataContext;
                string sql = $"select * from Attendance,{sd.DetailCourse.StudentListUrl} where stulisturl='{sd.DetailCourse.StudentListUrl}' and Attendance.stunum={sd.DetailCourse.StudentListUrl}.stunum";

                await Task.Run(() =>
                {

                    OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                    while (reader.Read())
                    {
                        Arrive a = new Arrive();
                        a.StuNum = reader["Attendance.stunum"].ToString();
                        a.StuName = reader["stuname"].ToString();
                        a.CourseNum = reader["coursenum"].ToString();
                        a.CourseTime = reader["coursetime"].ToString();
                        a.ArriveState = Convert.ToInt32(reader["arrivestate"].ToString());
                        this.Dispatcher.Invoke(() =>
                        {
                            attendancelist.Add(a);
                        });
                    }
                    reader.Close();
                    AccessDBHelper.CloseConnectDB();
                });
                this.atten.ItemsSource = attendancelist;
                this.arrive.ItemsSource = (from n in attendancelist select n.CourseTime).ToList().Distinct();
                this.arrive.SelectedIndex = 0;
            }
        }

        private void arrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrive.ItemsSource != null)
            {
                this.atten.ItemsSource = attendancelist.Select(x => x.CourseTime == arrive.SelectedItem.ToString()).ToList();
            }
        }
    }
}
