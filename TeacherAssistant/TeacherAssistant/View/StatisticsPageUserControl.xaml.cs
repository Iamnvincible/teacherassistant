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
    /// Interaction logic for StatisticsPageUserControl.xaml
    /// </summary>
    public partial class StatisticsPageUserControl : UserControl
    {
        StatisticsPageViewModel spvm;
        public StatisticsPageUserControl()
        {
            InitializeComponent();
            spvm = new StatisticsPageViewModel();
            this.DataContext = spvm;
        }
        protected void CouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var course = courselist.SelectedItem as ClassDetail;
            if (course != null)
            {
                UserControl p = new StatisticsDetailPageUserControl();
                StatisticsDetailViewModel sdvm = new StatisticsDetailViewModel();
                sdvm.DetailCourse = course;
                p.DataContext = sdvm;
                this.Content = p;
            }
        }

        private void SortCourseName(object sender, RoutedEventArgs e)
        {
            spvm.Cd = new ObservableCollection<ClassDetail>(spvm.Cd.OrderBy(x => x.CourseName).ToList());
        }

        private void Statistics(object sender, RoutedEventArgs e)
        {
            //Page p = new StatisticsDetailPage();
            //this.NavigationService.Navigate(p);
            UserControl p = new StatisticsDetailPageUserControl();
            this.Content = p;
            //this.gri.Children.Clear();
            //this.gri.Children.Add(p);
        }
    }
}
