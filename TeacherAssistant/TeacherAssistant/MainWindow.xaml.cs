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
            string teachernum = TeacherNumberBox.Text.Trim();
            string filename = String.Format("{0}.mdb", teachernum);
            FileInfo dbfile = new FileInfo(filename);
            string filePath = new DirectoryInfo(".").FullName + "\\" + filename;
            if (dbfile.Exists)
            {
                Debug.WriteLine("File exists");
                //AccessDBHelper.ConnectDB(filePath);
                List<ClassDetail> classtable = TableHelp.GetClassTable(this.TeacherNumberBox.Text);
                string itempatten = "insert into classtable (classname,lastweeks, classtype, subject, stuclasses,studentlistrul, classday , classtime) values";
                foreach (var item in classtable)
                {
                    string insert = String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", item.Name, item.LastWeeks, item.ClassType, item.Subject, item.StuClasses, item.StudentListURL, item.Day, item.Time);
                    string sql = itempatten + insert;
                    AccessDBHelper.CreateTable(sql,filePath);
                }
                //todo 连接数据库，读取数据
            }
            else
            {
                //创建数据库
                AccessDBHelper.CreateDB(filePath);
                //AccessDBHelper.ConnectDB(filePath);
                AccessDBHelper.CreateTable("create table classtable (id autoincrement primary key, classname TEXT(100), lastweeks TEXT(100), classtype TEXT(100), subject TEXT(100), stuclasses text(100) ,studentlistrul TEXT(100), classday TEXT(100), classtime text(100))",filePath);
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
