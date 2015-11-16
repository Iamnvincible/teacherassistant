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
using TeacherAssistant.DataBase;
using System.Data.OleDb;
using System.Data;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for ClassListWindow.xaml
    /// </summary>
    public partial class ClassListWindow : Window
    {
        public ClassListWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AccessDBHelper.ConnectDB(App.Databasefilepath);
            string sql = "select id from classtable";
            OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            DataTable dt = new DataTable();
            DataRow dr;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                DataColumn dc;
                dc = new DataColumn(reader.GetName(i));
                dt.Columns.Add(dc);

            }
            while (reader.Read())
            {
                dr = dt.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dr[reader.GetName(i)] = reader[reader.GetName(i)].ToString();
                }
            }
        }
    }
}
