using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using TeacherAssistant.DataBase;
using MahApps.Metro.Controls;
using TeacherAssistant.Model;
using System.Collections.ObjectModel;

namespace TeacherAssistant.View
{
    /// <summary>
    /// Interaction logic for ClassListWindow.xaml
    /// </summary>
    public partial class ClassListWindow
    {
        ObservableCollection<TeachClassStuA> grid = new ObservableCollection<TeachClassStuA>();
        public ClassListWindow()
        {
            InitializeComponent();

            grid.Add(new TeachClassStuA()
            {
                Num = 1,
                Subject = "计算机科学与技术",
                StudentNum = "2013211429",
                Name = "林杰",
                Sex = "男",
                ClassNum = "0491302",
                Year = 2013,
                ClassState = "正常"

            });
            //getdata();
           // stulist.DataContext = grid;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            List<TeachClassStuA> studatalist = new List<TeachClassStuA>();
            //AccessDBHelper.ConnectDB(App.Databasefilepath);
            string sql = "select * from A041518124736";
            OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            while (reader.Read())
            {
                TeachClassStuA tcsa = new TeachClassStuA();
                tcsa.Num = (int)reader["num"];
                tcsa.Subject = reader["subject"].ToString();
                tcsa.StudentNum = reader["stunum"].ToString();
                tcsa.Name = reader["stuname"].ToString();
                tcsa.Sex = reader["sex"].ToString();
                tcsa.ClassNum = reader["classnum"].ToString();
                tcsa.ClassState = reader["classstate"].ToString();
                studatalist.Add(tcsa);
            }
            //DataTable dt = new DataTable();
            //DataRow dr;
            //for (int i = 0; i < reader.FieldCount; i++)
            //{
            //    DataColumn dc;
            //    dc = new DataColumn(reader.GetName(i));
            //    dt.Columns.Add(dc);

            //}
            //while (reader.Read())
            //{
            //    dr = dt.NewRow();
            //    for (int i = 0; i < reader.FieldCount; i++)
            //    {
            //        dr[reader.GetName(i)] = reader[reader.GetName(i)].ToString();
            //     }
            //}

        }
        void getdata()
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + App.Databasefilepath);
            OleDbDataAdapter da = new OleDbDataAdapter("Select * from A041518124736", conn);
            DataSet ds = new DataSet();
            DataSet quertDs = new DataSet();
            OleDbCommandBuilder cmd = new OleDbCommandBuilder(da);
            conn.Open();
            da = new OleDbDataAdapter("Select * from A041518124736", conn);
            //da.InsertCommand = cmd.GetInsertCommand();
            //da.UpdateCommand = cmd.GetUpdateCommand();
            //da.DeleteCommand = cmd.GetDeleteCommand();
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds);
            conn.Close();
            stulist.DataContext = ds.Tables[0];
        }
    }
}
