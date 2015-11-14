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
using TeacherAssistant.NetWork;
using TeacherAssistant.View;

namespace TeacherAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Loginbutton_Click(object sender, RoutedEventArgs e)
        {
            //TableHelp.GetJxbStuList("A041518124736");
            //TableHelp.GetJxbStuList("SJ021516266569");
            //TableHelp.GetClassTable("040317");
            TableHelp.GetClassTable(this.TeacherNumberBox.Text);
            //TableHelp.GetClassTable("041212"); 
            Window clw = new ClassListWindow();
            clw.Show();
            this.Close();
        }
    }
}
