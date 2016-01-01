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
using DotNetSpeech;
using System.Diagnostics;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for CoursePage.xaml
    /// </summary>
    public partial class CoursePage : Page
    {
        SpVoice speech = new SpVoice();
        public CoursePageViewModel vm;
        List<TeachClassStu> studatalist;
        Brush brush = default(Brush);
        int selected = 0;
        string stulisturl = string.Empty;
        /// <summary>
        /// 构造函数，数据绑定VM
        /// </summary>
        public CoursePage()
        {
            InitializeComponent();
            vm = new CoursePageViewModel();
            studatalist = new List<TeachClassStu>();
            this.DataContext = vm;
            this.combo.ItemsSource = vm.coursename;
            this.combo.SelectionChanged += Combo_SelectionChanged;
        }
        /// <summary>
        /// 课程下拉列表改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.current.Text = this.combo.SelectedItem as string;
            //课程选定，更改combox时间
            //combot.ItemsSource
            var a = App.classtable;
            var b = from n in a where n.CourseName == this.current.Text select n;
            var c = b.ToList();
            List<string> d = new List<string>();
            foreach (var item in c)
            {
                string isoddweek = String.Empty;
                if (item.LastWeeks.All(i => i % 2 == 0) && item.LastWeeks.Length > 4)
                {
                    isoddweek = "双周";
                }
                else if (item.LastWeeks.All(i => i % 2 != 0) && item.LastWeeks.Length > 4)
                {
                    isoddweek = "单周";
                }
                d.Add(item.CourseDay + item.CourseTime + isoddweek);
            }
            this.combot.ItemsSource = d;
            this.namelist.Visibility = Visibility.Collapsed;
            this.pleaseselect.Visibility = Visibility.Visible;
            this.onestudent.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 上一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SetBorder(new SolidColorBrush(Color.FromArgb(255, 204, 204, 204)));
            selected = selected <= 0 ? studatalist.Count - 1 : selected - 1;
            this.onestudent.DataContext = studatalist[selected];
            if (selected != 0 && voice.IsChecked == true)
            {
                speek(stuname.Text);
            }
            SetBorder(new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)));
        }
        /// <summary>
        /// 下一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SetBorder(new SolidColorBrush(Color.FromArgb(255, 204, 204, 204)));
            selected = selected >= studatalist.Count - 1 ? 0 : selected + 1;
            this.onestudent.DataContext = studatalist[selected];
            if (selected != 0 && voice.IsChecked == true)
            {
                speek(stuname.Text);
            }
            SetBorder(new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)));
        }
        /// <summary>
        /// 时间段改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var a = App.classtable;
            if (this.combot.SelectedItem != null)
            {
                string daytime = this.combot.SelectedItem as string;
                string day = daytime.Substring(0, 3);
                string time = daytime.Substring(3, 3);
                var c = from n in a where n.CourseName == this.current.Text && n.CourseDay == day && n.CourseTime == time select n;
                var selected = c.ToList();
                if (selected.Count > 1 && daytime.Contains("周"))
                {
                    if (daytime.Contains("双"))
                    {
                        if (selected[0].LastWeeks.All(x => x % 2 == 0))
                        {
                            studatalist = AccessDBHelper.GetStuList(selected[0].StudentListUrl);
                            stulisturl = selected[0].StudentListUrl;
                        }
                        else
                        {
                            studatalist = AccessDBHelper.GetStuList(selected[1].StudentListUrl);
                            stulisturl = selected[1].StudentListUrl;
                        }
                    }
                    else
                    {
                        if (selected[0].LastWeeks.All(x => x % 2 != 0))
                        {
                            studatalist = AccessDBHelper.GetStuList(selected[0].StudentListUrl);
                            stulisturl = selected[0].StudentListUrl;
                        }
                        else
                        {
                            studatalist = AccessDBHelper.GetStuList(selected[1].StudentListUrl);
                            stulisturl = selected[1].StudentListUrl;
                        }
                    }
                }
                else
                {
                    studatalist = AccessDBHelper.GetStuList(selected[0].StudentListUrl);
                    stulisturl = selected[0].StudentListUrl;
                }
                onestudent.Visibility = Visibility.Visible;
                if (studatalist != null)
                    studatalist.Sort(new TeachClassStu());
                this.namelist.ItemsSource = studatalist;
                this.onestudent.DataContext = studatalist;
                this.pleaseselect.Visibility = Visibility.Collapsed;
                this.namelist.Visibility = Visibility.Visible;
            }

        }
        /// <summary>
        /// 点名按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void callbtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.homework.Visibility = Visibility.Collapsed;
            this.welcome.Visibility = Visibility.Collapsed;
            this.calltheroll.Visibility = Visibility.Visible;
            if (this.current.Text != "现在没有课" && this.combot.Text == "")
            {
                studatalist = AccessDBHelper.GetStuList(vm.currentcourse.StudentListUrl);
                stulisturl = vm.currentcourse.StudentListUrl;
                onestudent.Visibility = Visibility.Visible;
                if (studatalist != null)
                    studatalist.Sort(new TeachClassStu());
                this.namelist.ItemsSource = studatalist;
                this.onestudent.DataContext = studatalist;
            }
            else if (this.current.Text == "现在没有课")
            {
                this.pleaseselect.Visibility = Visibility.Visible;
            }



            //AccessDBHelper.ConnectDB(App.Databasefilepath);
            //string sql = "select * from A041518124736";
            //await Task.Run(() =>
            //{
            //    OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            //    while (reader.Read())
            //    {
            //        TeachClassStu tcsa = new TeachClassStu();
            //        tcsa.StuNum = reader["stunum"].ToString();
            //        tcsa.StuName = reader["stuname"].ToString();
            //        tcsa.Sex = reader["sex"].ToString();
            //        tcsa.Subject = reader["subject"].ToString();
            //        tcsa.ClassNum = reader["classnum"].ToString();
            //        tcsa.ClassState = reader["classstate"].ToString();
            //        tcsa.ClassType = reader["classtype"].ToString();
            //        tcsa.Num = Convert.ToInt32(reader["num"].ToString());
            //        studatalist.Add(tcsa);
            //    }
            //    reader.Close();
            //    AccessDBHelper.CloseConnectDB();
            //});
            //await Task.Delay(1000);
        }
        /// <summary>
        /// 作业按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void homeworkbtn_Click(object sender, RoutedEventArgs e)
        {
            this.welcome.Visibility = Visibility.Collapsed;
            this.calltheroll.Visibility = Visibility.Collapsed;
            this.homework.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 欢迎
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void welcomebtn_Click(object sender, RoutedEventArgs e)
        {
            this.welcome.Visibility = Visibility.Visible;
            this.calltheroll.Visibility = Visibility.Collapsed;
            this.homework.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        ///点到按钮三个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Arrived_Click(object sender, RoutedEventArgs e)
        {
            Button colorbtn = sender as Button;
            SetOne(colorbtn.Background);
        }
        /// <summary>
        /// 可视化树的查找
        /// </summary>
        /// <param name="relate"></param>
        /// <param name="type"></param>
        /// <param name="resElement"></param>
        private void FindChildByType(DependencyObject relate, Type type, ref FrameworkElement resElement)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(relate); i++)
            {
                var el = VisualTreeHelper.GetChild(relate, i) as FrameworkElement;
                if (el.GetType() == type)
                {
                    resElement = el;
                    return;
                }
                else
                {
                    FindChildByType(el, type, ref resElement);
                }
            }
        }
        /// <summary>
        /// 设置一个按钮的颜色什么的
        /// </summary>
        /// <param name="scb"></param>
        private void SetOne(Brush scb)
        {
            string nu = this.stunumber.Text;
            var t = from n in studatalist where n.StuNum == nu select n;
            var btitem = this.namelist.ItemContainerGenerator.ContainerFromItem(t.First()) as ListViewItem;
            FrameworkElement b = default(FrameworkElement);
            FindChildByType(btitem, typeof(Button), ref b);
            Button theone = b as Button;
            theone.Background = scb;//new SolidColorBrush(Color.FromArgb(255, 50, 177, 108));
            selected = selected >= studatalist.Count - 1 ? 0 : selected + 1;
            this.onestudent.DataContext = studatalist[selected];
            if (selected != 0 && voice.IsChecked == true)
            {
                speech.Speak(" ", SpeechVoiceSpeakFlags.SVSFlagsAsync);
                speech.Speak(stuname.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            }
            theone.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 204, 204, 204));
            theone.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            SetBorder(new SolidColorBrush(Color.FromArgb(255, 250, 0, 0)));
        }
        /// <summary>
        /// 设置按钮边框
        /// </summary>
        /// <param name="scb"></param>
        private void SetBorder(SolidColorBrush scb)
        {
            var btitemn = this.namelist.ItemContainerGenerator.ContainerFromItem(studatalist[selected]) as ListViewItem;
            FrameworkElement bn = default(FrameworkElement);
            FindChildByType(btitemn, typeof(Button), ref bn);
            Button theone = bn as Button;
            theone.BorderBrush = scb;// new SolidColorBrush(Color.FromArgb(255, 250, 0, 0));
        }
        /// <summary>
        /// 个人姓名按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stuname_Click(object sender, RoutedEventArgs e)
        {
            SetBorder(new SolidColorBrush(Color.FromArgb(255, 204, 204, 204)));
            TeachClassStu c = ((Button)sender).DataContext as TeachClassStu;
            var d = studatalist.Find(x => x.StuNum == c.StuNum);
            selected = c.Num - 1;
            this.onestudent.DataContext = studatalist[selected];
            if (voice.IsChecked == true)
            {
                speek(stuname.Text);
            }
            SetBorder(new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)));
        }
        /// <summary>
        /// 语音
        /// </summary>
        /// <param name="text">要说的文本</param>
        private void speek(string text)
        {
            speech.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }
        /// <summary>
        /// 将点名结果保存到数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToDB(object sender, RoutedEventArgs e)
        {

            List<Arrive> alist = new List<Arrive>();
            int arrivestate = 0;
            //课程
            string coursename = this.current.Text;
            ClassDetail course = App.classtable.Find(x => x.CourseName == coursename);
            string coursenum = course.CourseNum;
            string coursetime = "第" + App.WeekCount.ToString() + "周" + course.CourseDay + course.CourseTime;
            //学生
            //学生学号
            //到课状态，按钮背景色
            FrameworkElement b = default(FrameworkElement);
            for (int i = 0; i < studatalist.Count; i++)
            {
                ListViewItem btitem = this.namelist.ItemContainerGenerator.ContainerFromItem(studatalist[i]) as ListViewItem;
                FindChildByType(btitem, typeof(Button), ref b);
                Button theone = b as Button;
                if (theone.Background == this.Arrived.Background)
                {
                    arrivestate = 1;
                }
                else if (theone.Background == this.Out.Background)
                {
                    arrivestate = 2;
                }
                else if (theone.Background == this.Unknown.Background)
                {
                    arrivestate = 3;
                }
                else if (theone.Background == this.Late.Background)
                {
                    arrivestate = 4;
                }
                else
                {
                    arrivestate = 0;
                }
                alist.Add(new Arrive() { StuNum = studatalist[i].StuNum, CourseNum = coursenum, CourseTime = coursetime, ArriveState = arrivestate });
            }


            string[] SQLTransaction = new string[alist.Count];
            string itempatten = "insert into " + "Attendance" + " (stunum,coursenum,coursetime,arrivestate,stulisturl) values ";
            for (int trans = 0; trans < SQLTransaction.Length; trans++)
            {
                string insert = String.Format(itempatten + "('{0}','{1}','{2}','{3}','{4}')", alist[trans].StuNum, alist[trans].CourseNum, alist[trans].CourseTime, alist[trans].ArriveState, stulisturl);
                SQLTransaction[trans] = insert;
            }
            AccessDBHelper.Transaction(SQLTransaction, App.Databasefilepath);
            MessageBox.Show("点名数据成功保存");
        }

        private void stuname_MouseEnter(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            if (b != null)
            {
                brush = b.Foreground;
                Debug.WriteLine(brush.ToString());
                b.Foreground = b.Background;
                Debug.WriteLine(b.Foreground);
            }

        }

        private void stuname_MouseLeave(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            if (b != null)
            {
                b.Foreground = brush;
            }
        }
    }

}
