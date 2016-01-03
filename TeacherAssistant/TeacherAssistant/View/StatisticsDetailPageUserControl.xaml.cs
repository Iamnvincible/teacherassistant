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
        ObservableCollection<Arrive> saved = new ObservableCollection<Arrive>();
        int selectedsection = 0;
        public StatisticsDetailPageUserControl()
        {
            InitializeComponent();
            this.tab.SelectionChanged += Tab_SelectionChanged;
        }

        private void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int si = this.tab.SelectedIndex;
            Debug.WriteLine("选中{0}", si);
            if (si != selectedsection)
            {
                selectedsection=si;
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
        }

        private void Goback(object sender, RoutedEventArgs e)
        {
            this.Content = new StatisticsPageUserControl();
        }

        private async void getattendance()
        {
            if (attendancelist != atten.ItemsSource)
            {
                Debug.WriteLine("我我我我我我我");
                StatisticsDetailViewModel sd = (StatisticsDetailViewModel)this.DataContext;
                string sql = $"select stunum,stuname,classnum,coursetime,arrivestate from Attendance where stulisturl='{sd.DetailCourse.StudentListUrl}'";

                await Task.Run(() =>
                {
                    OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                    if (reader != null)
                    {
                        if (attendancelist != null)
                            this.Dispatcher.Invoke(() =>
                            {
                                attendancelist.Clear();
                            });
                        while (reader.Read())
                        {
                            Arrive a = new Arrive();
                            a.StuNum = reader["stunum"].ToString();
                            a.StuName = reader["stuname"].ToString();
                            a.ClassNum = reader["classnum"].ToString();
                            a.CourseTime = reader["coursetime"].ToString();
                            a.ArriveState = Convert.ToInt32(reader["arrivestate"].ToString());
                            this.Dispatcher.Invoke(() =>
                            {
                                attendancelist.Add(a);
                            });
                        }
                        reader.Close();
                    }
                    AccessDBHelper.CloseConnectDB();
                });
                this.atten.ItemsSource = attendancelist;
                this.arrive.ItemsSource = (from n in attendancelist select n.CourseTime).ToList().Distinct();
                saved = attendancelist;
                //this.arrive.SelectedIndex = 0;
                //this.atten.ItemsSource = attendancelist.Select(x => x.CourseTime == arrive.SelectedItem.ToString()).ToList();
            }
        }

        private void arrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrive.ItemsSource != null && arrive.SelectedItem != null)
            {
                //arrive.ItemsSource = null;
                this.atten.ItemsSource = null;
                attendancelist = new ObservableCollection<Arrive>((from n in saved where n.CourseTime == arrive.SelectedItem.ToString() select n).ToList());
                this.atten.ItemsSource = attendancelist;
                //this.atten.ItemsSource = attendancelist.Select(x => x.CourseTime == arrive.SelectedItem.ToString()).ToList();
            }
        }
    }
}
