﻿using System;
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
        List<ClassDetail> classtable = new List<ClassDetail>();
        public ClassListWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //getdata();
            getclasstable();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Loaddata.IsActive = true;
            List<TeachClassStu> studatalist = new List<TeachClassStu>();
            //AccessDBHelper.ConnectDB(App.Databasefilepath);
            string sql = "select * from A041518124736";
            await Task.Run(() =>
            {
                OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                while (reader.Read())
                {
                    TeachClassStu tcsa = new TeachClassStu();
                    tcsa.StuNum = reader["stunum"].ToString();
                    tcsa.StuName = reader["stuname"].ToString();
                    tcsa.Sex = reader["sex"].ToString();
                    tcsa.Subject = reader["subject"].ToString();
                    tcsa.ClassNum = reader["classnum"].ToString();
                    tcsa.ClassState = reader["classstate"].ToString();
                    tcsa.ClassType = reader["classtype"].ToString();
                    tcsa.Num=Convert.ToInt32(reader["num"].ToString());
                    studatalist.Add(tcsa);
                }
            });
            await Task.Delay(1000);
            studatalist.Sort(new TeachClassStu());
            this.stulist.DataContext = studatalist;
            this.Loaddata.IsActive = false;
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
        async void getclasstable()
        {
            string sql = "select * from classtable";
            await Task.Run(() =>
            {
                OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
                while (reader.Read())
                {
                    ClassDetail tcsa = new ClassDetail();
                    tcsa.CourseNum = reader["coursenum"].ToString();
                    tcsa.CourseName = reader["coursename"].ToString();
                    tcsa.Classroom = reader["classroom"].ToString();
                    tcsa.LastWeeks = Array.ConvertAll<string,int>(reader["lastweeks"].ToString().Split(','),s=>int.Parse(s));
                    tcsa.ClassType = reader["classtype"].ToString();
                    tcsa.Subject = reader["subject"].ToString();
                    tcsa.StuClassNum = reader["stuclassnum"].ToString().Split(',');
                    tcsa.CourseDay = reader["courseday"].ToString();
                    tcsa.CourseTime = reader["coursetime"].ToString();
                    classtable.Add(tcsa);
                }
            });
            this.table.ItemsSource = classtable;
        }
    }
}
