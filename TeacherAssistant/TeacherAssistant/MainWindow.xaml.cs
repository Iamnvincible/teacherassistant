using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TeacherAssistant.DataBase;
using TeacherAssistant.Model;
using TeacherAssistant.NetWork;
using TeacherAssistant.View;

namespace TeacherAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
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
                await Task.Delay(1000);
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
            string filename = String.Format("{0}.mdb", teachernum);
            FileInfo dbfile =  new FileInfo(filename);
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
                AccessDBHelper.CreateTable("create table classtable (id autoincrement primary key, classname TEXT(100), lastweeks TEXT(100), classtype TEXT(100), subject TEXT(100), stuclasses text(100) ,studentlisturl TEXT(100), classday TEXT(100), classtime text(100))", filePath);
                List<ClassDetail> classtable = TableHelp.GetClassTable(teachernum);
                if (classtable==null)
                {
                    return false;
                }
                string itempattentb = "insert into classtable (classname,lastweeks, classtype, subject, stuclasses,studentlisturl, classday , classtime) values";
                foreach (var item in classtable)
                {
                    string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", item.Name, item.LastWeeks, item.ClassType, item.Subject, item.StuClasses, item.StudentListURL, item.Day, item.Time);
                    string sql = itempattentb + insert;
                    AccessDBHelper.CreateTable(sql, filePath);
                }
                //学生名单表们
                var stuurls = from url in classtable where url.StudentListURL != null select url.StudentListURL;
                //url一样的删掉，获得这课的学生名单号。
                List<string> disurls = stuurls.ToList();
                disurls = disurls.Distinct().ToList();
                for (int i = 0; i < disurls.Count; i++)
                {
                    //实验课学生名单获取
                    if (disurls[i].Substring(20).StartsWith("S"))
                    {
                        List<TeachClassStuSJ> stu = TableHelp.GetJxbStuList(disurls[i]) as List<TeachClassStuSJ>;
                        string tablename = disurls[i].Substring(20);
                        string[] SQLTransaction = new string[stu.Count];
                        string sql = "create table " + tablename + " (num Integer,subject TEXT(100),stunum TEXT(100),stuname TEXT(100),sex TEXT(100),classnum TEXT(100),classstate TEXT(100),classtype TEXT(100))";
                        AccessDBHelper.CreateTable(sql, filePath);
                        string itempatten = "insert into " + tablename + " (num, subject, stunum, stuname, sex, classnum, classstate, classtype) values ";
                        for (int trans = 0; trans < SQLTransaction.Length; trans++)
                        {
                            string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", stu[i].Num, stu[i].Subject, stu[i].StudentNum, stu[i].Name, stu[i].Sex, stu[i].ClassNum, stu[i].ClassState, stu[i].ClassType);
                        }
                        AccessDBHelper.Transaction(SQLTransaction, filePath);
                        return true;
                        //foreach (var item in stu)
                        //{
                        //    string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", item.Num, item.Subject, item.StudentNum, item.Name, item.Sex, item.ClassNum, item.ClassState, item.ClassType);
                        //    string sqlinsert = itempatten + insert;
                        //    AccessDBHelper.CreateTable(sqlinsert, filePath);
                        //}
                    }
                    //理论课学生名单获取
                    else if (disurls[i].Substring(20).StartsWith("A") || disurls[i].Substring(20).StartsWith("R"))
                    {
                        List<TeachClassStuA> stu = TableHelp.GetJxbStuList(disurls[i]) as List<TeachClassStuA>;
                        string tablename = disurls[i].Substring(20);
                        string[] SQLTransaction = new string[stu.Count];
                        string sql = "create table " + tablename + " (num Integer,subject TEXT(100),stunum TEXT(100),stuname TEXT(100),sex TEXT(100),classnum TEXT(100),classtype TEXT(100),classstate TEXT(100))";
                        AccessDBHelper.CreateTable(sql, filePath);
                        string itempatten = "insert into " + tablename + " (num, subject, stunum, stuname, sex, classnum, classstate, classtype) values ";
                        for (int trans = 0; trans < SQLTransaction.Length; trans++)
                        {
                            string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", stu[i].Num, stu[i].Subject, stu[i].StudentNum, stu[i].Name, stu[i].Sex, stu[i].ClassNum, stu[i].Year, stu[i].ClassState);
                        }
                        AccessDBHelper.Transaction(SQLTransaction, filePath);
                        return true;
                        //foreach (var item in stu)
                        //{
                        //    string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", item.Num, item.Subject, item.StudentNum, item.Name, item.Sex, item.ClassNum, item.Year, item.ClassState);
                        //    string sqlinsert = itempatten + insert;
                        //    AccessDBHelper.CreateTable(sqlinsert, filePath);
                        //}
                    }
                    //超出我的预料啊
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }

    }
}
