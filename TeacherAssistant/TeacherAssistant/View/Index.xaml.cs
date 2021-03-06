﻿using System;
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
using WpfPageTransitions;

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
            //IndexPage page1 = new IndexPage();
            IndexPageUserControl u1 = new IndexPageUserControl();
            //naviFrame.ShowPage(page1);
            naviFrame.ShowPage(u1);
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var n = e.Source as Ellipse;
            Debug.WriteLine(n);
            //Page p = new Page();
            //p = new IndexPage();
            UserControl p = new UserControl();
            p = new IndexPageUserControl();
            switch (n.Name)
            {
                case "index":
                    //p = new IndexPage();
                    p = new IndexPageUserControl();
                    n.Opacity = 1;
                    this.course.Opacity = 0.5;
                    this.statistics.Opacity = 0.5;
                    this.other.Opacity = 0.5;
                    break;
                case "course":
                    //p = new CoursePage();
                    p = new CoursePageUserControl();
                    n.Opacity = 1;
                    this.index.Opacity = 0.5;
                    this.statistics.Opacity = 0.5;
                    this.other.Opacity = 0.5;
                    break;
                case "statistics":
                    //p = new StatisticsPage();
                    p = new StatisticsPageUserControl();
                    n.Opacity = 1;
                    this.index.Opacity = 0.5;
                    this.course.Opacity=0.5;
                    this.other.Opacity = 0.5;
                    break;
                case "other":
                    //p = new MorePage();
                    p = new MorePageUserControl();
                    n.Opacity = 1;
                    this.index.Opacity = 0.5;
                    this.course.Opacity = 0.5;
                    this.statistics.Opacity = 0.5;
                    break;
                default: break;
            }
            //naviFrame.NavigationService.Navigate(p);
            naviFrame.ShowPage(p);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow();
            m.Show();
            this.Close();
        }
    }
}
