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
        int selected = 0;
        public CoursePage()
        {
            InitializeComponent();
            vm = new CoursePageViewModel();
            studatalist = new List<TeachClassStu>();
            this.DataContext = vm;
            this.combo.ItemsSource = vm.coursename;
            this.combo.SelectionChanged += Combo_SelectionChanged;
        }

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
                d.Add(item.CourseDay + item.CourseTime);
            }
            this.combot.ItemsSource = d;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClassListWindow clw = new ClassListWindow();
            clw.Show();
        }


        /// <summary>
        /// 上一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            selected = selected <= 0 ? studatalist.Count - 1 : selected - 1;
            this.onestudent.DataContext = studatalist[selected];
            speech.Speak(" ", SpeechVoiceSpeakFlags.SVSFlagsAsync);
            speech.Speak(stuname.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }
        /// <summary>
        /// 下一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            selected = selected >= studatalist.Count - 1 ? 0 : selected + 1;
            this.onestudent.DataContext = studatalist[selected];
            speech.Speak(" ", SpeechVoiceSpeakFlags.SVSFlagsAsync);
            speech.Speak(stuname.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }

        private void combot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var a = App.classtable;
            if (this.combot.SelectedItem != null)
            {
                string daytime = this.combot.SelectedItem as string;
                string day = daytime.Substring(0, 3);
                string time = daytime.Substring(3);
                var c = from n in a where n.CourseName == this.current.Text && n.CourseDay == day && n.CourseTime == time select n;
                var selected = c.ToList();
                studatalist = AccessDBHelper.GetStuList(selected[0].StudentListUrl);
                onestudent.Visibility = Visibility.Visible;
                if (studatalist != null)
                    studatalist.Sort(new TeachClassStu());
                this.namelist.ItemsSource = studatalist;
                this.onestudent.DataContext = studatalist;
                this.pleaseselect.Visibility = Visibility.Collapsed;
            }

        }

        private void callbtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.homework.Visibility = Visibility.Collapsed;
            this.welcome.Visibility = Visibility.Collapsed;
            this.calltheroll.Visibility = Visibility.Visible;
            if (this.current.Text != "现在没有课" && this.combot.Text == "")
            {
                studatalist = AccessDBHelper.GetStuList(vm.currentcourse.StudentListUrl);
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
        private void homeworkbtn_Click(object sender, RoutedEventArgs e)
        {
            this.welcome.Visibility = Visibility.Collapsed;
            this.calltheroll.Visibility = Visibility.Collapsed;
            this.homework.Visibility = Visibility.Visible;
        }

        private void welcomebtn_Click(object sender, RoutedEventArgs e)
        {
            this.welcome.Visibility = Visibility.Visible;
            this.calltheroll.Visibility = Visibility.Collapsed;
            this.homework.Visibility = Visibility.Collapsed;
        }

        private void Arrived_Click(object sender, RoutedEventArgs e)
        {
            //Button stuname=(Button)this.namelist.Items.GetItemAt(0);
            //Button stuname=(Button)VisualTreeHelper.GetChild(this.namelist, 0);
            string nu = this.stunumber.Text;
            var t = from n in studatalist where n.StuNum == nu select n;
            var btitem = this.namelist.ItemContainerGenerator.ContainerFromItem(t.First()) as ListViewItem;
            FrameworkElement b = default(FrameworkElement);
            FindChildByType(btitem, typeof(Button), ref b);
            Button theone = b as Button;
            theone.Background = new SolidColorBrush(Color.FromArgb(255, 50, 177, 108));
            selected = selected >= studatalist.Count - 1 ? 0 : selected + 1;
            this.onestudent.DataContext = studatalist[selected];
            if (selected != 0)
            {

                speech.Speak(" ", SpeechVoiceSpeakFlags.SVSFlagsAsync);
                speech.Speak(stuname.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            }

            //foreach (var item in this.namelist.Items)
            //{
            //    var el = this.namelist.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            //    if (el != null && el is ListViewItem)
            //    {
            //        ListViewItem lbItem = el as ListViewItem;
            //        FrameworkElement efind = default(FrameworkElement);
            //        FindChildByType(lbItem, typeof(Button), ref efind);
            //        if (efind is Button)
            //        {
            //            Button btn = efind as Button;

            //            btn.Background = new SolidColorBrush(Colors.Red);
            //        }
            //    }
            //}
        }

        private void Out_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Unknown_Click(object sender, RoutedEventArgs e)
        {

        }

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
    }

}
