using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherAssistant.Model;

namespace TeacherAssistant.NetWork
{
    public static class TableHelp
    {
        public static List<TeachClassStu> GetJxbStuList(string jsbnum)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            List<TeachClassStu> stulist = new List<TeachClassStu>();
            string url = @"http://jwzx.cqupt.edu.cn/showJxbStuList.php?jxb=" + jsbnum;
            MemoryStream ms = new MemoryStream(client.DownloadData(url));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.GetEncoding("gb2312"));
            HtmlDocument docStockContext = new HtmlDocument();
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
                var sex=dd[4].InnerText.Trim();
                var classnum=dd[5].InnerText.Trim();
                var year= Convert.ToInt32(dd[6].InnerText.Trim());
                var state=dd[7].InnerText.Trim();
                Debug.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}",num,subject,stunum,name,sex,classnum,year,state);
                TeachClassStu t = new TeachClassStu { Num = num, Subject = subject, StudentNum = stunum, Name = name, Sex = sex, ClassNum = classnum, Year = year, ClassState = state };
                stulist.Add(t);
            }

            //

            return stulist;
            //byte[] page = client.DownloadData(url);
            // string content = System.Text.Encoding.GetEncoding("gb2312").GetString(page);
            //Debug.Write(content);
        }
    }
}
