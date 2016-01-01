using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TeacherAssistant.Model;
using TeacherAssistant.ViewModel;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        StatisticsPageViewModel spvm;
        public StatisticsPage()
        {
            InitializeComponent();
            spvm = new StatisticsPageViewModel();
            this.DataContext = spvm;
            //courselist.ItemsSource = spvm.cd;
        }
        protected void CouseDoubleClick(object sender,MouseButtonEventArgs e)
        {
            var course=courselist.SelectedItem as ClassDetail;
            if(course!=null)
            MessageBox.Show(course.StudentListUrl+course.CourseName);
        }

        private void SortCourseName(object sender, RoutedEventArgs e)
        {
            spvm.Cd=new ObservableCollection<ClassDetail>(spvm.Cd.OrderBy(x=>x.CourseName).ToList());
        }
    }
}
