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
        int speechrate = 0;
        int volume = 100;
        public CoursePageViewModel vm;
        List<TeachClassStu> studatalist;
        int selected = 0;
        public CoursePage()
        {
            InitializeComponent();
            vm = new CoursePageViewModel();
            this.DataContext = vm;
            this.combo.ItemsSource = vm.coursename;
            this.combo.SelectionChanged += Combo_SelectionChanged;
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.current.Text = this.combo.SelectedItem as string;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClassListWindow clw = new ClassListWindow();
            clw.Show();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            studatalist = new List<TeachClassStu>();
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
                reader.Close();
                AccessDBHelper.CloseConnectDB();
            });
            //await Task.Delay(1000);
            studatalist.Sort(new TeachClassStu());
            this.namelist.ItemsSource = studatalist;
            this.onestudent.DataContext = studatalist;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            selected = selected <= 0 ? studatalist.Count - 1 : selected - 1;
            this.onestudent.DataContext = studatalist[selected];
            speech.Speak(" ", SpeechVoiceSpeakFlags.SVSFlagsAsync);
            speech.Speak(stuname.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            selected = selected >= studatalist.Count - 1 ? 0 : selected + 1;
            this.onestudent.DataContext = studatalist[selected];
            speech.Speak(" ", SpeechVoiceSpeakFlags.SVSFlagsAsync);
            speech.Speak(stuname.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }
    }

}
