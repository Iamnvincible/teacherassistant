﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TeacherAssistant.DataBase;
using TeacherAssistant.Model;
using TeacherAssistant.NetWork;
using TeacherAssistant.View;
using MahApps.Metro.Controls;

namespace TeacherAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private async void Loginbutton_Click(object sender, RoutedEventArgs e)
        {
            logoimage.Visibility = Visibility.Collapsed;
            message.Text = "登录中";
            Downloading.Visibility = Visibility.Visible;
            string teachernum = TeacherNumberBox.Text.Trim();
            bool a = await Task.Run(() =>
            {
                return get(teachernum);
            });
            if (a)
            {
                await Task.Delay(100);
                Downloading.Visibility = Visibility.Collapsed;
                Window clw = new ClassListWindow();
                clw.Show();
                this.Close();
            }
            else
            {
                Downloading.Visibility = Visibility.Collapsed;
                logoimage.Visibility = Visibility.Visible;
                message.Text = "出现错误!";
            }

        }
        bool get(string teachernum)
        {
            string filename = String.Format("{0}.mdb", teachernum);//以教师号命名数据库文件
            FileInfo dbfile = new FileInfo(filename);
            string filePath = new DirectoryInfo(".").FullName + "\\" + filename;
            App.Databasefilepath = filePath;
            if (dbfile.Exists)
            {
                return true;
            }
            else
            {
                //创建数据库
                AccessDBHelper.CreateDB(filePath);
                //AccessDBHelper.ConnectDB(filePath);
                AccessDBHelper.CreateTable("create table classtable (id autoincrement primary key, coursenum TEXT(10), coursename TEXT(100), classroom TEXT(100), lastweeks TEXT(100), classtype text(50) ,subject TEXT(80), stuclassnum TEXT(100), stulisturl TEXT(50),courseday TEXT(10),coursetime TEXT(10))", filePath);
                List<ClassDetail> classtable = TableHelp.GetClassTable(teachernum);
                if (classtable == null)
                {
                    return false;
                }
                string itempattentb = "insert into classtable (coursenum, coursename, classroom, lastweeks , classtype ,subject, stuclassnum, stulisturl,courseday,coursetime) values";
                foreach (var item in classtable)
                {
                    string LastWeeks = "";
                    string StuClassNum = "";
                    for (int i = 0; i < item.LastWeeks.Length; i++)
                    {
                        //if (item.LastWeeks[i] == -1)
                        //{
                        //    LastWeeks = "单周";
                        //    break;
                        //}
                        //if (item.LastWeeks[i] == -2)
                        //{
                        //    LastWeeks = "双周";
                        //    break;
                        //}
                        if (i != 0)
                            LastWeeks += ",";
                        LastWeeks += item.LastWeeks[i];
                    }
                    for (int i = 0; i < item.StuClassNum.Length; i++)
                    {

                        if (i != 0)
                            StuClassNum += ",";
                        StuClassNum += item.StuClassNum[i];

                    }
                    string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", item.CourseNum, item.CourseName, item.Classroom, LastWeeks, item.ClassType, item.Subject, StuClassNum, item.StudentListUrl, item.CourseDay, item.CourseTime);
                    string sql = itempattentb + insert;
                    AccessDBHelper.CreateTable(sql, filePath);
                }
                //学生名单表们
                var stuurls = from url in classtable where url.StudentListUrl != null select url.StudentListUrl;
                //url一样的删掉，获得这课的学生名单号。
                List<string> disurls = stuurls.ToList();
                disurls = disurls.Distinct().ToList();
                for (int i = 0; i < disurls.Count; i++)
                {
                    //实验课学生名单获取
                    if (disurls[i] != null)
                    {
                        List<TeachClassStu> stu = TableHelp.GetJxbStuList(disurls[i]);
                        string tablename = disurls[i];
                        string[] SQLTransaction = new string[stu.Count];
                        string sql = "create table " + tablename + " (stunum TEXT(50) primary key, stuname TEXT(100),sex TEXT(4),subject TEXT(100),classnum TEXT(100),classstate TEXT(50),classtype TEXT(50),num autoincrement)";
                        AccessDBHelper.CreateTable(sql, filePath);
                        string itempatten = "insert into " + tablename + " (stunum, stuname, sex, subject, classnum, classstate, classtype) values ";
                        for (int trans = 0; trans < SQLTransaction.Length; trans++)
                        {
                            string insert = String.Format(itempatten + "('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", stu[trans].StuNum, stu[trans].StuName, stu[trans].Sex, stu[trans].Subject, stu[trans].ClassNum, stu[trans].ClassState, stu[trans].ClassType);
                            SQLTransaction[trans] = insert;
                        }
                        AccessDBHelper.Transaction(SQLTransaction, filePath);
                    }
                    //理论课学生名单获取
                    //else if (disurls[i].Substring(20).StartsWith("A") || disurls[i].Substring(20).StartsWith("R"))
                    //{
                    //    List<TeachClassStuA> stu = TableHelp.GetJxbStuList(disurls[i]) as List<TeachClassStuA>;
                    //    string tablename = disurls[i].Substring(20);
                    //    string[] SQLTransaction = new string[stu.Count];
                    //    string sql = "create table " + tablename + " (num Integer,subject TEXT(100),stunum TEXT(100),stuname TEXT(100),sex TEXT(100),classnum TEXT(100),inrollyear TEXT(4),classstate TEXT(100))";
                    //    AccessDBHelper.CreateTable(sql, filePath);
                    //    string itempatten = "insert into " + tablename + " (num, subject, stunum, stuname, sex, classnum,inrollyear,classstate) values ";
                    //    for (int trans = 0; trans < SQLTransaction.Length; trans++)
                    //    {
                    //        string insert = String.Format(itempatten + "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", stu[trans].Num, stu[trans].Subject, stu[trans].StudentNum, stu[trans].Name, stu[trans].Sex, stu[trans].ClassNum, stu[trans].Year, stu[trans].ClassState);
                    //        SQLTransaction[trans] = insert;

                    //    }
                    //    AccessDBHelper.Transaction(SQLTransaction, filePath);
                    //}
                    //超出我的预料啊
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }

    }
}
