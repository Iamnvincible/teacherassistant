using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        public static object GetJxbStuList(string jsbnum)
        {
            WebClient client = new WebClient();
            MemoryStream ms;
            HtmlDocument doc = new HtmlDocument();
            HtmlDocument docStockContext = new HtmlDocument();
            if (jsbnum.ToUpper().StartsWith("S"))
            {
                string urlnew = @"http://jwzx.cqupt.edu.cn/new/labkebiao/showjxbStuList.php?jxb=" + jsbnum;
                List<TeachClassStuSJ> stulist = new List<TeachClassStuSJ>();
                GetJxbStuListSJ(out ms, doc, docStockContext, client, urlnew, ref stulist);
                return stulist;

            }
            else if (jsbnum.ToUpper().StartsWith("A")|| jsbnum.ToUpper().StartsWith("R"))
            {
                string url = @"http://jwzx.cqupt.edu.cn/showJxbStuList.php?jxb=" + jsbnum;
                List<TeachClassStuA> stulist = new List<TeachClassStuA>();
                GetJxbStuListA(out ms, doc, docStockContext, client, url, ref stulist);
                return stulist;
            }
            else
            {
                return null;

            }
        }
        public static List<ClassDetail> GetClassTable(string teachernum)
        {
            WebClient client = new WebClient();
            MemoryStream ms;
            HtmlDocument doc = new HtmlDocument();
            HtmlDocument docStockContext = new HtmlDocument();
            string url = @"http://jwzx.cqupt.edu.cn/new/labkebiao/showteakebiao2.php?tid=" + teachernum;
            ms = new MemoryStream(client.DownloadData(url));
            doc.Load(ms, Encoding.GetEncoding("gbk"));
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
                    if (item[j].InnerHtml.Trim().Length!=6)
                    {
                        string innerhtml = item[j].InnerHtml.Replace("&nbsp;", "");
                        string temp = innerhtml.Replace("学生名单", "");
                        int k = (innerhtml.Length - temp.Length) / 4;
                        item[j].InnerHtml = item[j].InnerHtml.Replace("&nbsp;", "");
                        var inner = item[j].SelectNodes("./text()");
                        for (int l = 0; l < k; l++)
                        {
                            int offset = inner.Count / k * l;
                            string classname = inner[0 + offset].InnerText.Trim();
                            string classroom = inner[1 + offset].InnerText.Trim();
                            string lastweeks = inner[2 + offset].InnerText.Trim();
                            var classtype = item[j].SelectNodes("./font")[l*2].InnerText.Trim();// inner[3 + offset].InnerText;
                            string subject = inner[4 + offset].InnerText.Trim();
                            string stuclasses = inner[5 + offset].InnerText.Trim();
                            string listurl = item[j].SelectNodes("..//a[@href]")[l].Attributes["href"].Value.Trim();
                            ClassDetail cd = new ClassDetail { Classroom = classroom, Name = classname, LastWeeks = lastweeks, Subject = subject, ClassType = classtype, StuClasses = stuclasses, Day = date[i], Time = times[i], StudentListURL = listurl };
                            classtable.Add(cd);
                        }


                    }
                }
            }
            return classtable;
        }
        /// <summary>
        /// 获得实验课学生名单
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="doc"></param>
        /// <param name="docStockContext"></param>
        /// <param name="client"></param>
        /// <param name="url">网页地址</param>
        /// <param name="stulist">学生名单集合</param>
        private static void GetJxbStuListSJ(out MemoryStream ms, HtmlDocument doc, HtmlDocument docStockContext, WebClient client, string url, ref List<TeachClassStuSJ> stulist)
        {
            ms = new MemoryStream(client.DownloadData(url));
            doc.Load(ms, Encoding.GetEncoding("gbk"));
            var table = doc.DocumentNode.SelectSingleNode("/html/body/table[1]").InnerHtml;
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
                var num = Convert.ToInt32(dd[0].InnerText.Trim());
                var subject = dd[1].InnerText.Trim();
                var stunum = Convert.ToInt32(dd[2].InnerText.Trim());
                var name = dd[3].InnerText.Trim();
                var sex = dd[4].InnerText.Trim();
                var classnum = dd[5].InnerText.Trim();
                var classtype = dd[7].InnerText.Trim();
                var state = dd[6].InnerText.Trim();
                Debug.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", num, subject, stunum, name, sex, classnum, classtype, state);
                TeachClassStuSJ t = new TeachClassStuSJ { Num = num, Subject = subject, StudentNum = stunum, Name = name, Sex = sex, ClassNum = classnum, ClassType = classtype, ClassState = state };
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
        private static void GetJxbStuListA(out MemoryStream ms, HtmlDocument doc, HtmlDocument docStockContext, WebClient client, string url, ref List<TeachClassStuA> stulist)
        {
            ms = new MemoryStream(client.DownloadData(url));
            doc.Load(ms, Encoding.GetEncoding("gb2312"));
            var table = doc.DocumentNode.SelectSingleNode("/table[1]").InnerHtml;
            docStockContext.LoadHtml(table);
            //取得表头
            HtmlNodeCollection nodeHeaders = docStockContext.DocumentNode.SelectNodes("/tr[1]/td");
            //输出表头
            foreach (var item in nodeHeaders)
            {
                Debug.Write(item.InnerText + " ");
            }
            //获得名单
            var list = docStockContext.DocumentNode.SelectNodes("/tr");
            list.RemoveAt(0);
            foreach (var item in list)
            {
                var dd = item.SelectNodes("td");
                var num = Convert.ToInt32(dd[0].InnerText.Trim());
                var subject = dd[1].InnerText.Trim();
                var stunum = Convert.ToInt32(dd[2].InnerText.Trim());
                var name = dd[3].InnerText.Trim();
                var sex = dd[4].InnerText.Trim();
                var classnum = dd[5].InnerText.Trim();
                var year = Convert.ToInt32(dd[6].InnerText.Trim());
                var state = dd[7].InnerText.Trim();
                Debug.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", num, subject, stunum, name, sex, classnum, year, state);
                TeachClassStuA t = new TeachClassStuA { Num = num, Subject = subject, StudentNum = stunum, Name = name, Sex = sex, ClassNum = classnum, Year = year, ClassState = state };
                stulist.Add(t);
            }
        }
    }
}
