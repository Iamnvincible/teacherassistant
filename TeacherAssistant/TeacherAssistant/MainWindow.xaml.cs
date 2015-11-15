using ADOX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using TeacherAssistant.DataBase;
using TeacherAssistant.Model;
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
           flag: string teachernum = TeacherNumberBox.Text.Trim();
            string filename = String.Format("{0}.mdb", teachernum);
            FileInfo dbfile = new FileInfo(filename);
            string filePath = new DirectoryInfo(".").FullName + "\\" + filename;
            if (dbfile.Exists)
            {
                Debug.WriteLine("File exists");
                //AccessDBHelper.ConnectDB(filePath);

                List<ClassDetail> classtable = TableHelp.GetClassTable(this.TeacherNumberBox.Text);
                
                string itempattentb = "insert into classtable (classname,lastweeks, classtype, subject, stuclasses,studentlistrul, classday , classtime) values";
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
                    if (disurls[i].Substring(20).StartsWith("S"))
                    {
                        List<TeachClassStuSJ> stu = TableHelp.GetJxbStuList(disurls[i]) as List<TeachClassStuSJ>;
                        string tablename = disurls[i].Substring(20);
                        string sql = "create table "+tablename+" (num Integer,subject TEXT(100),stunum TEXT(100),stuname TEXT(100),sex TEXT(100),classnum TEXT(100),classstate TEXT(100),classtype TEXT(100))";
                        AccessDBHelper.CreateTable(sql, filePath);
                        string itempatten = "insert into " + tablename + " (num, subject, stunum, stuname, sex, classnum, classstate, classtype) values ";
                        foreach (var item in stu)
                        {
                            string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", item.Num, item.Subject, item.StudentNum, item.Name, item.Sex, item.ClassNum, item.ClassState, item.ClassType);
                            string sqlinsert = itempatten + insert;
                            AccessDBHelper.CreateTable(sqlinsert, filePath);
                        }
                    }
                    else if(disurls[i].Substring(20).StartsWith("A")|| disurls[i].Substring(20).StartsWith("R"))
                    {
                        List<TeachClassStuA> stu = TableHelp.GetJxbStuList(disurls[i]) as List<TeachClassStuA>;
                        string tablename = disurls[i].Substring(20);
                        string sql = "create table " + tablename + " (num Integer,subject TEXT(100),stunum TEXT(100),stuname TEXT(100),sex TEXT(100),classnum TEXT(100),classtype TEXT(100),classstate TEXT(100))";
                        AccessDBHelper.CreateTable(sql, filePath);
                        string itempatten = "insert into " + tablename + " (num, subject, stunum, stuname, sex, classnum, classstate, classtype) values ";
                        foreach (var item in stu)
                        {
                            string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", item.Num, item.Subject, item.StudentNum, item.Name, item.Sex, item.ClassNum,item.Year,item.ClassState);
                            string sqlinsert = itempatten + insert;
                            AccessDBHelper.CreateTable(sqlinsert, filePath);
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                // AccessDBHelper.CreateTable("create table classtable (id autoincrement primary key, classname TEXT(100), lastweeks TEXT(100), classtype TEXT(100), subject TEXT(100), stuclasses text(100) ,studentlistrul TEXT(100), classday TEXT(100), classtime text(100))", filePath);

                //todo 连接数据库，读取数据
            }
            else
            {
                //创建数据库
                AccessDBHelper.CreateDB(filePath);
                //AccessDBHelper.ConnectDB(filePath);
                AccessDBHelper.CreateTable("create table classtable (id autoincrement primary key, classname TEXT(100), lastweeks TEXT(100), classtype TEXT(100), subject TEXT(100), stuclasses text(100) ,studentlistrul TEXT(100), classday TEXT(100), classtime text(100))", filePath);
                goto flag;
                //todo 存入数据
                //内网获取课表
                //List<ClassDetail> classtable = TableHelp.GetClassTable(this.TeacherNumberBox.Text);
            }
            //TableHelp.GetClassTable("041212"); 
            Window clw = new ClassListWindow();
            clw.Show();
            this.Close();

        }
    }
}
