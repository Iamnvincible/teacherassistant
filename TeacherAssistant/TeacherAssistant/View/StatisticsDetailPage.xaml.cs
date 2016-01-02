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

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for StatisticsDetailPage.xaml
    /// </summary>
    public partial class StatisticsDetailPage : Page
    {
        public StatisticsDetailPage()
        {
            InitializeComponent();
        }

        private void Goback(object sender, RoutedEventArgs e)
        {
            var s = this.NavigationService.CanGoBack;
            if (s)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
