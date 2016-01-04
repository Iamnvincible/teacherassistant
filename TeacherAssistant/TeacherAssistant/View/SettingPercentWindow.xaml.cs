using System;
using System.Collections.Generic;
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
using MahApps.Metro.Controls;
using System.Windows.Shapes;
using System.Configuration;
namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for SettingPercentWindow.xaml
    /// </summary>
    public partial class SettingPercentWindow : MetroWindow
    {
        public SettingPercentWindow()
        {
            InitializeComponent();
            
            this.arrive.Text = ConfigurationManager.AppSettings["arrive"];
            this.homework.Text = ConfigurationManager.AppSettings["homework"];
            this.addition.Text = ConfigurationManager.AppSettings["addition"];
            this.exam.Text = ConfigurationManager.AppSettings["exam"];

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int arrive, homework, add, exam;
            if ((int.TryParse(this.arrive.Text, out arrive)) && (int.TryParse(this.homework.Text, out homework)) && (int.TryParse(this.addition.Text, out add)) && (int.TryParse(this.exam.Text, out exam)))
            {
                if (arrive + homework + add + exam == 100)
                {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["arrive"].Value = arrive.ToString();
                    cfa.AppSettings.Settings["homework"].Value = homework.ToString();
                    cfa.AppSettings.Settings["addition"].Value = add.ToString();
                    cfa.AppSettings.Settings["exam"].Value = exam.ToString();
                    cfa.Save();
                    this.Close();
                }
                else
                    MessageBox.Show("请检查输入！");
            }
            else
            {
                MessageBox.Show("请检查输入！");
            }
        }
    }
}
