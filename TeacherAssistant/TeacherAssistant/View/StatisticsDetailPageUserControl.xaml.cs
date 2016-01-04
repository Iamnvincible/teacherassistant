using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        List<Arrive> attendancelist = new List<Arrive>();
        List<Arrive> saved = new List<Arrive>();
        List<Homework> readhomelist = new List<Homework>();
        List<Homework> currenthomelist = new List<Homework>();
        int selectedsection = 0;
        public StatisticsDetailPageUserControl()
        {
            InitializeComponent();
            this.tab.SelectionChanged += Tab_SelectionChanged;
        }

        private async void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int si = this.tab.SelectedIndex;
            Debug.WriteLine("选中{0}", si);
            if (si != selectedsection)
            {
                selectedsection = si;
                switch (si)
                {
                    case 0:
                        break;
                    case 1:
                        await Task.Delay(1000);
                        this.progess.Visibility = Visibility.Visible;
                        //var task = Task.Factory.StartNew(() => showloading());
                        //var task3 = Task.Factory.StartNew(() => gethomework());
                        //var task2 = Task.Factory.StartNew(() => closeloading());
                        //await gethomework();
                        //task.Wait();
                        //task3.Wait();
                        //task2.Wait();


                        await Task.Run(() =>
                            {
                                gethomework();
                            });

                        this.progess.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        await Task.Delay(300);
                        this.progess.IsActive = true;
                        getattendance();
                        this.progess.IsActive = false;
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
        /// <summary>
        /// 获取考勤记录
        /// </summary>
        private async void getattendance()
        {
            if (attendancelist != atten.ItemsSource)
            {
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
        /// <summary>
        /// 考勤时间下拉列表选择，筛选数据，Linq大法好
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (arrive.ItemsSource != null && arrive.SelectedItem != null)
            {
                //arrive.ItemsSource = null;
                this.atten.ItemsSource = null;
                attendancelist = (from n in saved where n.CourseTime == arrive.SelectedItem.ToString() select n).ToList();
                this.atten.ItemsSource = attendancelist;
                //this.atten.ItemsSource = attendancelist.Select(x => x.CourseTime == arrive.SelectedItem.ToString()).ToList();
            }
        }
        /// <summary>
        /// 获取作业成绩记录
        /// </summary>
        private void gethomework()
        {
            if (currenthomelist != homeworklistview.ItemsSource)
            {
                //this.progess.Visibility = Visibility.Visible;
                StatisticsDetailViewModel sd = new StatisticsDetailViewModel();
                this.Dispatcher.Invoke(() =>
                {
                    sd = (StatisticsDetailViewModel)this.DataContext;
                });

                string sql = $"select stuname,stunum,classnum,score,hcount from Homework where stulisturl='{sd.DetailCourse.StudentListUrl}';";

                OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                if (reader != null)
                {
                    if (attendancelist != null)
                        this.Dispatcher.Invoke(() =>
                        {
                            currenthomelist.Clear();
                        });
                    while (reader.Read())
                    {
                        Homework a = new Homework();
                        a.StuNum = reader["stunum"].ToString();
                        a.StuName = reader["stuname"].ToString();
                        a.ClassNum = reader["classnum"].ToString();
                        a.Score = Convert.ToDecimal(reader["score"].ToString());
                        a.Count = Convert.ToInt32(reader["hcount"].ToString());
                        this.Dispatcher.Invoke(() =>
                        {
                            currenthomelist.Add(a);
                        });
                    }
                    reader.Close();
                }
                this.Dispatcher.Invoke(() =>
                {

                    AccessDBHelper.CloseConnectDB();
                    this.homeworklistview.ItemsSource = currenthomelist;
                    this.homeworktime.ItemsSource = (from n in currenthomelist select n.Count).ToList().Distinct();
                    readhomelist = currenthomelist;
                });
            }
        }
        /// <summary>
        /// 作业时间下拉逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void homeworktime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (homeworktime.ItemsSource != null && homeworktime.SelectedItem != null)
            {
                this.homeworklistview.ItemsSource = null;
                currenthomelist = (from n in readhomelist where n.Count.ToString() == homeworktime.SelectedItem.ToString() select n).ToList();
                this.homeworklistview.ItemsSource = currenthomelist;
            }
        }
        /// <summary>
        /// 添加作业成绩
        /// </summary>
        private async void edithome_Click(object sender, RoutedEventArgs e)
        {
            StatisticsDetailViewModel sd = (StatisticsDetailViewModel)this.DataContext;
            if (this.edithome.Content.ToString() == "添加成绩")
            {
                if (this.currenthomelist != null)
                    this.currenthomelist.Clear();
                //string sql = $"select stuname,stunum,classnum,score,count FROM homework stulisturl='{sd.DetailCourse.StudentListUrl}';";
                string sql = $"select stuname,stunum,classnum from {sd.DetailCourse.StudentListUrl}";
                //this.progess.IsActive = true;
                //await Task
                await Task.Delay(1000);
                await Task.Run(() =>
                {
                    OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Homework a = new Homework();
                            a.StuNum = reader["stunum"].ToString();
                            a.StuName = reader["stuname"].ToString();
                            a.ClassNum = reader["classnum"].ToString();
                            a.Score = 0;
                            a.Count = this.homeworktime.Items.Count + 1;
                            a.Stulisturl = sd.DetailCourse.StudentListUrl;
                            this.Dispatcher.Invoke(() =>
                            {
                                currenthomelist.Add(a);
                            });
                        }
                        reader.Close();
                    }
                    AccessDBHelper.CloseConnectDB();
                    this.Dispatcher.Invoke(() =>
                    {
                        this.homeworklistview.ItemsSource = null;
                        this.homeworklistview.ItemsSource = currenthomelist;
                        //this.homeworktime.Items.Add(currenthomelist[0].Count);
                        this.homeworktime.IsEnabled = false;
                        this.edithome.Content = "保存成绩";
                    });
                });
                //this.progess.IsActive = false;
            }
            else
            {
                //保存成绩
                string[] SQLTransaction = new string[currenthomelist.Count - 1];
                string itempatten = $"insert into Homework (stuname,stunum,classnum,score,coursenum,hcount,stulisturl) values ";
                for (int i = 0; i < SQLTransaction.Length; i++)
                {
                    string insert = itempatten + $"('{currenthomelist[i].StuName}','{currenthomelist[i].StuNum}','{currenthomelist[i].ClassNum}','{currenthomelist[i].Score}','{sd.DetailCourse.CourseNum}','{currenthomelist[0].Count}','{currenthomelist[0].Stulisturl}')";
                    SQLTransaction[i] = insert;
                }
                AccessDBHelper.Transaction(SQLTransaction, App.Databasefilepath);
                this.homeworktime.IsEnabled = true;
                this.readhomelist.AddRange(currenthomelist);
                this.edithome.Content = "添加成绩";
                this.homeworktime.ItemsSource = null;
                this.homeworktime.ItemsSource = (from n in readhomelist select n.Count).ToList().Distinct();
                MessageBox.Show("已保存");
            }
        }
        private void showloading()
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.progess.Visibility = Visibility.Visible;
            });
        }
        private void closeloading()
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.progess.Visibility = Visibility.Collapsed;
            });
        }
    }
}
