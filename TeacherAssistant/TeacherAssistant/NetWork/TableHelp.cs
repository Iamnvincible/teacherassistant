using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeacherAssistant.Model;

namespace TeacherAssistant.NetWork
{
    public static class TableHelp
    {
        /// <summary>
        /// 根据教学班号获得该教学班学生名单
        /// </summary>
        /// <param name="jsbnum">教学班号</param>
        /// <returns>返回学生名单对象，需要类型转换List<T></returns>
        public static List<TeachClassStu> GetJxbStuList(string jsbnum)
        {
            WebClient client = new WebClient();
            MemoryStream ms;
            HtmlDocument doc = new HtmlDocument();
            HtmlDocument docStockContext = new HtmlDocument();
            if (jsbnum != null)
            {
                string urlnew = @"http://jwzx.cqupt.edu.cn/new/labkebiao/showStuList.php?jxb=" + jsbnum;
                List<TeachClassStu> stulist;
                GetJxbStuList(out ms, doc, docStockContext, client, urlnew, out stulist);
                return stulist;

            }
            //if (jsbnum.Substring(20).ToUpper().StartsWith("S"))
            //{
            //    string urlnew = @"http://jwzx.cqupt.edu.cn/new/labkebiao/" + jsbnum;
            //    List<TeachClassStu> stulist;
            //    GetJxbStuListSJ(out ms, doc, docStockContext, client, urlnew, out stulist);
            //    return stulist;

            //}
            //else if (jsbnum.Substring(20).ToUpper().StartsWith("A") || jsbnum.ToUpper().StartsWith("R"))
            //{
            //    string url = @"http://jwzx.cqupt.edu.cn/new/labkebiao/" + jsbnum;
            //    List<TeachClassStu> stulist = new List<TeachClassStu>();
            //    GetJxbStuListA(out ms, doc, docStockContext, client, url, ref stulist);
            //    return stulist;
            //}
            else
            {
                return null;

            }
        }
        /// <summary>
        /// 获取教师课表
        /// </summary>
        /// <param name="teachernum">教师编号</param>
        /// <returns>返回课程列表</returns>
        public static List<ClassDetail> GetClassTable(string teachernum)
        {
            MemoryStream ms;
            HtmlDocument doc = new HtmlDocument();
            HtmlDocument docStockContext = new HtmlDocument();
            string url = @"http://jwzx.cqupt.edu.cn/new/labkebiao/showteakebiao2.php?tid=" + teachernum;
            byte[] downloadbytes = DownLoad(url);
            if (downloadbytes.Length > 1 && downloadbytes[0] != 0)
            {
                ms = new MemoryStream(downloadbytes);
                doc.Load(ms, Encoding.GetEncoding("gbk"));
            }
            else
            {
                return null;
            }
            var table = doc.DocumentNode.SelectSingleNode("/html/body/div/table[1]").InnerHtml;
            docStockContext.LoadHtml(table);
            //获得星期的列表
            HtmlNodeCollection nodeHeaders = docStockContext.DocumentNode.SelectNodes("/tr[1]/td");
            //移除第一个空格
            nodeHeaders.RemoveAt(0);
            //星期列表
            string[] date = new string[7];
            for (int i = 0; i < nodeHeaders.Count; i++)
            {
                date[i] = nodeHeaders[i].InnerHtml;
            }
            //上课时间段数组
            string[] times = new string[6];
            //获取表格每一列
            HtmlNodeCollection list = docStockContext.DocumentNode.SelectNodes("/tr");
            //移除表头
            list.RemoveAt(0);
            //大课表
            List<ClassDetail> classtable = new List<ClassDetail>();
            //循环每个上课时间段
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i].SelectNodes("td");//获取行
                times[i] = item[0].InnerText.Trim();//时间段
                for (int j = 1; j < item.Count; j++)//从星期一到星期天
                {
                    var iii = item[j].InnerHtml.Trim().LastIndexOf('&');
                    if (item[j].InnerHtml.Trim().Length != 6)
                    {
                        string innerhtml = item[j].InnerHtml.Replace("&nbsp;", "");
                        string temp = innerhtml.Replace("学生名单", "");
                        int k = (innerhtml.Length - temp.Length) / 4;
                        item[j].InnerHtml = item[j].InnerHtml.Replace("&nbsp;", "");
                        var inner = item[j].SelectNodes("./text()");
                        for (int l = 0; l < k; l++)
                        {
                            int offset = inner.Count / k * l;
                            string coursenum = inner[0 + offset].InnerText.Trim().Substring(0, 6);
                            string coursename = inner[0 + offset].InnerText.Trim().Substring(6);
                            string classroom = inner[1 + offset].InnerText.Trim();
                            int[] lastweeks = TransLastWeeks(inner[2 + offset].InnerText.Trim());
                            var classtype = item[j].SelectNodes("./font")[l * 2].InnerText.Trim();
                            string subject = inner[4 + offset].InnerText.Trim();
                            string[] stuclasses = TransClasslist(inner[5 + offset].InnerText.Trim());
                            string listurl = item[j].SelectNodes("..//a[@href]")[l].Attributes["href"].Value.Trim().Substring(20);
                            ClassDetail cd = new ClassDetail { CourseNum = coursenum, CourseName = coursename, Classroom = classroom, LastWeeks = lastweeks, ClassType = classtype, Subject = subject, StuClassNum = stuclasses, StudentListUrl = listurl, CourseDay = date[j - 1], CourseTime = times[i] };
                            classtable.Add(cd);
                        }
                    }
                }
            }
            return classtable;
        }
        /// <summary>
        /// 获得学生名单
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="doc"></param>
        /// <param name="docStockContext"></param>
        /// <param name="client"></param>
        /// <param name="url">网页地址</param>
        /// <param name="stulist">学生名单集合</param>
        private static void GetJxbStuList(out MemoryStream ms, HtmlDocument doc, HtmlDocument docStockContext, WebClient client, string url, out List<TeachClassStu> stulist)
        {
            byte[] downloadbytes = DownLoad(url);
            stulist = new List<TeachClassStu>();
            string table;
            if (downloadbytes.Length > 1 && downloadbytes[0] != 0)
            {
                ms = new MemoryStream(downloadbytes);
                if (url[59]==('S'))
                {
                    doc.Load(ms, Encoding.GetEncoding("gbk"));
                    table = doc.DocumentNode.SelectSingleNode("/html/body/table[1]").InnerHtml;
                }
                else
                {
                    doc.Load(ms, Encoding.GetEncoding("gb2312"));
                    table = doc.DocumentNode.SelectSingleNode("/table[1]").InnerHtml;
                }
            }
            else
            {
                ms = null;
                stulist = null;
                return;
            }
            //ms = new MemoryStream(DownLoad(url));
            //doc.Load(ms, Encoding.GetEncoding("gbk"));

            docStockContext.LoadHtml(table);
            //取得表头
            HtmlNodeCollection nodeHeaders = docStockContext.DocumentNode.SelectNodes("/tr[1]/td");
            //输出表头
            foreach (var item in nodeHeaders)
            {
                Debug.Write(item.InnerText + " ");
            }
            //获得名单
            HtmlNodeCollection list = docStockContext.DocumentNode.SelectNodes("/tr");
            list.RemoveAt(0);
            foreach (var item in list)
            {
                var dd = item.SelectNodes("td");
                //var num = Convert.ToInt32(dd[0].InnerText.Trim());
                var stunum = dd[2].InnerText.Trim();
                var name = dd[3].InnerText.Trim();
                var sex = dd[4].InnerText.Trim();
                var subject = dd[1].InnerText.Trim();
                var classnum = dd[5].InnerText.Trim();
                var classtype = dd[7].InnerText.Trim();
                var state = dd[6].InnerText.Trim();
                Debug.WriteLine("{0},{1},{2},{3},{4},{5},{6}", stunum, name, sex, subject, classnum, classtype, state);
                TeachClassStu t = new TeachClassStu { StuNum = stunum, Subject = subject, StuName = name, Sex = sex, ClassNum = classnum, ClassType = classtype, ClassState = state };
                stulist.Add(t);
            }
        }
        /// <summary>
        /// 获得理论课学生名单
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="doc"></param>
        /// <param name="docStockContext"></param>
        /// <param name="client"></param>
        /// <param name="url">网页地址</param>
        /// <param name="stulist">学生名单集合</param>
        //private static void GetJxbStuListA(out MemoryStream ms, HtmlDocument doc, HtmlDocument docStockContext, WebClient client, string url, ref List<TeachClassStu> stulist)
        //{
        //    ms = new MemoryStream(client.DownloadData(url));
        //    doc.Load(ms, Encoding.GetEncoding("gb2312"));
        //    var table = doc.DocumentNode.SelectSingleNode("/table[1]").InnerHtml;
        //    docStockContext.LoadHtml(table);
        //    //取得表头
        //    HtmlNodeCollection nodeHeaders = docStockContext.DocumentNode.SelectNodes("/tr[1]/td");
        //    //输出表头
        //    foreach (var item in nodeHeaders)
        //    {
        //        Debug.Write(item.InnerText + " ");
        //    }
        //    //获得名单
        //    var list = docStockContext.DocumentNode.SelectNodes("/tr");
        //    list.RemoveAt(0);
        //    foreach (var item in list)
        //    {
        //        var dd = item.SelectNodes("td");
        //        var num = Convert.ToInt32(dd[0].InnerText.Trim());
        //        var subject = dd[1].InnerText.Trim();
        //        var stunum = dd[2].InnerText.Trim();
        //        var name = dd[3].InnerText.Trim();
        //        var sex = dd[4].InnerText.Trim();
        //        var classnum = dd[5].InnerText.Trim();
        //        //var year = Convert.ToInt32(dd[6].InnerText.Trim());
        //        var state = dd[7].InnerText.Trim();
        //        Debug.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", num, subject, stunum, name, sex, classnum, year, state);
        //        TeachClassStuA t = new TeachClassStuA { Num = num, Subject = subject, StudentNum = stunum, Name = name, Sex = sex, ClassNum = classnum, Year = year, ClassState = state };
        //        stulist.Add(t);
        //    }
        //}
        /// <summary>
        /// 下载网页内容
        /// </summary>
        /// <param name="url">教学班号</param>
        /// <returns>网页内容的byte[]</returns>
        private static byte[] DownLoad(string url)
        {
            Task<byte[]> task;
            task = new Task<byte[]>(() =>
            {
                try
                {

                    WebClient wc = new WebClient();
                    return wc.DownloadData(url);
                }
                catch (Exception)
                {

                    byte[] error = { 0 };
                    return error;
                }
            });
            task.Start();

            return task.Result.ToArray();
        }
        /// <summary>
        /// 把教务在线的上课周变成一个数组
        /// </summary>
        /// <param name="rawweeks"></param>
        /// <returns>上课周int 数组</returns>
        private static int[] TransLastWeeks(string rawweeks)
        {
            if (rawweeks == "单周" || rawweeks == "双周")
            {
                int[] singleweeks = { -1 };
                return singleweeks;
            }
            if (rawweeks == "双周")
            {
                int[] doubleweeks = { -2 };
                return doubleweeks;
            }
            else
            {
                List<int> weeks = new List<int>();
                string pattern = @"\d{1,}-\d{1,}|\d{1,}";
                Regex r = new Regex(pattern);
                MatchCollection matches = r.Matches(rawweeks);
                foreach (Match item in matches)
                {
                    if (item.Value.Contains("-"))
                    {
                        Regex rr = new Regex(@"\d{1,}");
                        MatchCollection ma = rr.Matches(item.Value);
                        for (int i = Convert.ToInt32(ma[0].Value); i <=Convert.ToInt32(ma[1].Value); i++)
                        {
                            weeks.Add(i);
                        }
                    }
                    else
                    {
                        weeks.Add(Convert.ToInt32(item.Value));
                    }
                }
                return weeks.ToArray();

            }
        }
        /// <summary>
        /// 教务在线教学班班级号转换成数组
        /// </summary>
        /// <param name="rawclasses"></param>
        /// <returns>班级编号数组</returns>
        private static string[] TransClasslist(string rawclasses)
        {
            List<string> classlist = new List<string>();
            Regex r = new Regex(@"\d{1,}");
            MatchCollection mc = r.Matches(rawclasses);
            string front = "";
            foreach (Match item in mc)
            {
                if (item.Value.Length > 2)
                {
                    front = item.Value;
                    continue;
                }
                else
                {
                    classlist.Add(front + item.Value);
                }
            }
            return classlist.ToArray();
        }
    }
}
