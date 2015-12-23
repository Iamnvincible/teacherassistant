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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeacherAssistant.Model;
using TeacherAssistant.ViewModel;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for CoursePage.xaml
    /// </summary>
    public partial class CoursePage : Page
    {
        public CoursePageViewModel vm;
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
    }
}
