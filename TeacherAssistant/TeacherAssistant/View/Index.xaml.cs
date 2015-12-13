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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Diagnostics;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : MetroWindow
    {
        public Index()
        {
            InitializeComponent();
            IndexPage page1 = new IndexPage();
            naviFrame.NavigationService.Navigate(page1);
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var n = e.Source as Ellipse;
            Debug.WriteLine(n);
            Page p = new Page();
            p = new IndexPage();
            switch (n.Name)
            {
                case "index": p = new IndexPage(); break;
                case "course": p = new CoursePage(); break;
                case "statistics": p = new StatisticsPage(); break;
                case "other": p = new MorePage(); break;
                default: break;
            }
            naviFrame.NavigationService.Navigate(p);
        }
    }
}
