using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.DataBase;
using TeacherAssistant.Model;

namespace TeacherAssistant.ViewModel
{
    public class StatisticsDetailViewModel
    {
        private ClassDetail detailcourse;
        public ClassDetail DetailCourse
        {
            set
            {
                this.detailcourse = value;
            }
            get { return detailcourse; }
        }

        public ObservableCollection<Homework> Currenthomelist
        {
            get
            {
                return currenthomelist;
            }

            set
            {
                currenthomelist = value;
            }
        }

        ObservableCollection<Homework> currenthomelist;
        public StatisticsDetailViewModel(ClassDetail d)
        {
            this.DetailCourse = d;
            //set(d);
        }
         void set(ClassDetail d)
        {
            Currenthomelist =  gethomeworkasync(d.StudentListUrl);

        }
        private  ObservableCollection<Homework> gethomeworkasync(string url)
        {
            string sql = $"select stuname,stunum,classnum,score,hcount from Homework where stulisturl='{url}'";
            ObservableCollection<Homework> c = new ObservableCollection<Homework>();
            //return await Task.Run(() =>
            //{
            //    OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            //    if (reader != null)
            //    {
            //        while (reader.Read())
            //        {
            //            Homework a = new Homework();
            //            a.StuNum = reader["stunum"].ToString();
            //            a.StuName = reader["stuname"].ToString();
            //            a.ClassNum = reader["classnum"].ToString();
            //            a.Score = Convert.ToDecimal(reader["score"].ToString());
            //            a.Count = Convert.ToInt32(reader["hcount"].ToString());
            //            c.Add(a);
            //        }
            //        reader.Close();
            //        AccessDBHelper.CloseConnectDB();
            //    }

            //    return c;
            OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            if (reader != null)
            {
                while (reader.Read())
                {
                    Homework a = new Homework();
                    a.StuNum = reader["stunum"].ToString();
                    a.StuName = reader["stuname"].ToString();
                    a.ClassNum = reader["classnum"].ToString();
                    a.Score = Convert.ToDecimal(reader["score"].ToString());
                    a.Count = Convert.ToInt32(reader["hcount"].ToString());
                    c.Add(a);
                }
                reader.Close();
                AccessDBHelper.CloseConnectDB();
            }
            return c;
            //});

        }

    }
}
