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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeacherAssistant.ViewModel;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class IndexPageUserControl : UserControl
    {
        IndexPageViewModel ipvm;
        public IndexPageUserControl()
        {
            InitializeComponent();
            ipvm = new IndexPageViewModel();
            this.DataContext = ipvm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.pro.Visibility = Visibility.Visible;
            this.addpro.Visibility = Visibility.Collapsed;
        }

        private void pro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.pro.SelectedItem is string)
            {
                this.tips.Text = this.pro.SelectedItem as string;
            }
            this.pro.Visibility = Visibility.Collapsed;
            this.addpro.Visibility = Visibility.Visible;
        }
    }
}

