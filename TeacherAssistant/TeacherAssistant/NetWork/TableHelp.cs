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
        public static object GetJxbStuList(string jsbnum)
        {
            WebClient client = new WebClient();
            MemoryStream ms;
            HtmlDocument doc = new HtmlDocument();
            HtmlDocument docStockContext = new HtmlDocument();
            if (jsbnum.ToUpper().StartsWith("SJ"))
            {
                string urlnew = @"http://jwzx.cqupt.edu.cn/new/labkebiao/showjxbStuList.php?jxb=" + jsbnum;
                List<TeachClassStuSJ> stulist = new List<TeachClassStuSJ>();
                GetJxbStuListSJ(out ms, doc, docStockContext, client, urlnew, ref stulist);
                return stulist;

            }
            else if (jsbnum.ToUpper().StartsWith("A"))
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
                var classtype = dd[7].InnerText.Trim();
                var state = dd[6].InnerText.Trim();
                Debug.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", num, subject, stunum, name, sex, classnum, classtype, state);
                TeachClassStuSJ t = new TeachClassStuSJ { Num = num, Subject = subject, StudentNum = stunum, Name = name, Sex = sex, ClassNum = classnum, ClassType = classtype, ClassState = state };
                stulist.Add(t);
            }
        }
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
