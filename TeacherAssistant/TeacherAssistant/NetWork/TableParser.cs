using CenterCLR.Sgml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TeacherAssistant.NetWork
{
    public class TableParser : IDisposable
    {
        /// <summary>
        /// html table 解析器
        /// </summary>

        private List<HTMLTable> table;
        private XmlDocument doc;
        private Encoding objEncoding;
        private SgmlReader reader;
        private byte[] htmlBytes;

        private bool disposed = false;
        private Component component = new Component();

        /// <summary>
        /// 文本的编码格式
        /// </summary>
        public Encoding Encode
        {
            get { return objEncoding; }
            set { objEncoding = value; }
        }
        /// <summary>
        /// 文档中的表格
        /// </summary>
        public List<HTMLTable> Tables
        {
            get { return table; }
        }
        /// <summary>
        /// 提供文档中表格的索引访问
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>HTMLTable</returns>
        public HTMLTable this[int index]
        {
            get
            {
                try
                {
                    return table[index];
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// 提供文档中表格的索引访问
        /// </summary>
        /// <param name="index">table index</param>
        /// <param name="subindex">tr index</param>
        /// <returns>HTMLTr</returns>
        public HTMLTr this[int index, int subindex]
        {
            get
            {
                try
                {
                    return table[index].Rows[subindex];
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// 提供文档中表格的索引访问
        /// </summary>
        /// <param name="index">table index</param>
        /// <param name="subindex">tr index</param>
        /// <param name="subindex">td index</param>
        /// <returns>HTMLTd</returns>
        public HTMLTd this[int index, int subindex, int ssubindex]
        {
            get
            {
                try
                {
                    return table[index].Rows[subindex].Cells[ssubindex];
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private Stream outStream;
        /// <summary>
        /// 提供格式化后的标准html文档流，以供保存
        /// </summary>
        public Stream OutputStream
        {
            get { return outStream; }
            set { outStream = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public TableParser()
        {
            doc = new XmlDocument();
            table = new List<HTMLTable>();
            reader = new SgmlReader();
            reader.DocType = "strict";
            objEncoding = Encoding.Default;
            outStream = new MemoryStream();
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~TableParser()
        {
            try
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (outStream != null)
                {
                    outStream.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    component.Dispose();
                }
                disposed = true;

            }
        }

        /// <summary>
        /// 从文件或URL中获取加载dom
        /// </summary>
        /// <param name="filename">文件名或URL</param>
        /// <param name="encoding">编码</param>
        public void Load(string filename, Encoding encoding)
        {
            Stream readstream = new MemoryStream();
            if (Uri.IsWellFormedUriString(filename, UriKind.Absolute))
            {
                using (WebClient web = new WebClient())
                {
                    try
                    {
                        web.Encoding = encoding;
                        readstream = web.OpenRead(filename);
                        Load(readstream, encoding);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        readstream.Close();
                        web.Dispose();
                    }
                }
            }
            else if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        Load(fs, encoding);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }
            else
            {
                throw new Exception("试图访问不存在的文件");
            }
        }
        /// <summary>
        /// 从流设备中加载dom
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encode">编码</param>
        public void Load(Stream stream, Encoding encode)
        {
            using (StreamWriter output = new StreamWriter(outStream, encode))
            {
                using (XmlTextWriter writer = new XmlTextWriter(outStream, encode))
                {
                    try
                    {
                        reader.InputStream = new StreamReader(stream, encode);
                       // reader.WhitespaceHandling = WhitespaceHandling.None;
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();
                        //设置一个根节点
                        writer.WriteStartElement("root");
                        while (reader.Read())
                        {
                            //获取html节点
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "html")
                            {
                                XmlReader r = reader.ReadSubtree();
                            }
                            else
                            {
                                if (reader.Name == null)
                                {
                                    continue;
                                }
                                //写入所有table节点
                                switch (reader.Name.ToLower())
                                {
                                    case "table":
                                        if (reader.NodeType == XmlNodeType.Element)
                                            writer.WriteNode(reader, true);
                                        break;
                                    case "":
                                    default:
                                        continue;
                                }
                            }

                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Flush();
                        writer.Close();
                        htmlBytes = ((MemoryStream)outStream).GetBuffer();
                        //编码转换
                        if (objEncoding != encode)
                        {
                            htmlBytes = Encoding.Convert(encode, objEncoding, htmlBytes, 0, htmlBytes.Length);
                        }
                        string xmltext = ReplaceLowOrderASCIICharacters(htmlBytes);
                        doc.LoadXml(xmltext);
                        getTable();

                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        writer.Close();
                        output.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 从字符串中加载dom
        /// </summary>
        /// <param name="xmltext">文本</param>
        /// <param name="encode">编码</param>
        public void LoadXml(string xmltext, Encoding encode)
        {
            try
            {
                MemoryStream inputStream = new MemoryStream(encode.GetBytes(xmltext));
                Load(inputStream, encode);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 查找table
        /// </summary>
        private void getTable()
        {
            XmlNodeList tablelist = doc.GetElementsByTagName("table");
            foreach (XmlNode _table in tablelist)
            {
                table.Add(new HTMLTable(_table));
            }
        }
        /// <summary>
        /// 修复xml低位字符串无法解析的问题
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string ReplaceLowOrderASCIICharacters(byte[] buffer)
        {
            string tmp = objEncoding.GetString(buffer);
            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                    info.AppendFormat(" ", ss);//&#x{0:X};
                else info.Append(cc);
            }
            return info.ToString();
        }
    }
    /// <summary>
    /// 代表一个html表格
    /// </summary>
    public class HTMLTable : HTMLTag
    {
        private List<HTMLTr> tr;
        /// <summary>
        /// 代表表格中的行，tr
        /// </summary>
        public List<HTMLTr> Rows
        {
            get { return tr; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node">xmlnode</param>
        public HTMLTable(XmlNode node) : base(node)
        {
            tr = new List<HTMLTr>();
            getSubElement(node, Tag.tr);
            getDeep = false;
            foreach (XmlNode snode in subnodelist)
            {
                tr.Add(new HTMLTr(snode));
            }
        }
    }
    /// <summary>
    /// 代表一个table中的行
    /// </summary>
    public class HTMLTr : HTMLTag
    {

        private List<HTMLTd> td;
        /// <summary>
        /// 代表行中的单元格，td
        /// </summary>
        public List<HTMLTd> Cells
        {
            get { return td; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node">xmlnode</param>
        public HTMLTr(XmlNode node) : base(node)
        {
            td = new List<HTMLTd>();
            getSubElement(node, Tag.td);
            getDeep = false;
            foreach (XmlNode snode in subnodelist)
            {
                td.Add(new HTMLTd(snode));
            }
        }

    }
    /// <summary>
    /// 代表一个table中的单元格
    /// </summary>
    public class HTMLTd : HTMLTag
    {
        public HTMLTd(XmlNode node) : base(node)
        {
        }
    }
    /// <summary>
    /// 代表一个html标签节点
    /// </summary>
    public class HTMLTag : IDisposable
    {
        protected XmlNode node;
        protected bool getDeep = true;
        protected List<XmlNode> subnodelist;
        /// <summary>
        /// 标签中的文字
        /// </summary>
        public string Value
        {
            get
            {
                try
                {
                    return node.InnerText;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 标签中的html文本
        /// </summary>
        public string innerHTML
        {
            get
            {
                try
                {
                    return node.InnerXml;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 属性索引器
        /// </summary>
        /// <param name="attribute">指定属性名</param>
        /// <returns>属性值</returns>
        public string this[string attribute]
        {
            get
            {
                try
                {
                    return node.Attributes[attribute].Value;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 查找指定标签名的子节点
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <returns>List<HTMLTag></returns>
        public virtual List<HTMLTag> this[Tag tagname]
        {
            get
            {
                try
                {
                    getSubElement(node, tagname);
                    List<HTMLTag> tags = new List<HTMLTag>();
                    foreach (XmlNode _node in subnodelist)
                    {
                        tags.Add(new HTMLTag(_node));
                    }
                    return tags;
                }
                catch (Exception)
                {
                    return new List<HTMLTag>();
                }
            }
        }
        /// <summary>
        /// 根据tagname查找元素
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <returns>List<HTMLTag></returns>
        public virtual List<HTMLTag> GetElementsByTagName(Tag tagname)
        {
            return this[tagname];
        }

        private XmlNode nodecopy;
        /// <summary>
        /// 根据id查找元素
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>HTMLTag</returns>
        public virtual HTMLTag GetElementByID(string id)
        {
            HTMLTag ht = new HTMLTag(nodecopy);
            foreach (XmlNode subnode in nodecopy.ChildNodes)
            {
                try
                {
                    string nodeid = subnode.Attributes["id"].Value;
                    {
                        if (nodeid.ToLower() == id.ToLower())
                        {
                            nodecopy = node;
                            return new HTMLTag(subnode);
                        }
                        else
                        {
                            nodecopy = subnode;
                            return GetElementByID(id);
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return ht;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node">节点</param>
        public HTMLTag(XmlNode node)
        {
            this.node = node;
            this.nodecopy = node;
            subnodelist = new List<XmlNode>();
        }
        /// <summary>
        /// 查找子结点
        /// </summary>
        /// <param name="parentnode">父节点</param>
        /// <param name="tagname">标签名</param>
        protected void getSubElement(XmlNode parentnode, Tag tagname)
        {

            foreach (XmlNode subnode in parentnode.ChildNodes)
            {
                if (subnode.Name == tagname.ToString())
                {
                    subnodelist.Add(subnode);
                }
                if (getDeep && subnode.HasChildNodes)
                {
                    getSubElement(subnode, tagname);
                }
            }
        }

        private bool disposed = false;
        private Component component = new Component();

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    component.Dispose();
                }
                disposed = true;

            }
        }

    }
    /// <summary>
    /// html标签名枚举
    /// </summary>
    public enum Tag
    {
        a,
        abbr,
        acronym,
        address,
        applet,
        area,
        b,
        Base,
        basefont,
        bdo,
        big,
        blockquote,
        body,
        br,
        button,
        caption,
        center,
        cite,
        code,
        col,
        colgroup,
        dd,
        del,
        dfn,
        dir,
        div,
        dl,
        dt,
        em,
        fieldset,
        font,
        form,
        frame,
        frameset,
        h1,
        h2,
        h3,
        h4,
        h5,
        h6,
        head,
        hr,
        html,
        i,
        iframe,
        img,
        input,
        ins,
        isindex,
        kbd,
        label,
        legend,
        li,
        link,
        listing,
        map,
        menu,
        meta,
        noframes,
        noscript,
        Object,
        ol,
        optgroup,
        option,
        p,
        param,
        plaintext,
        pre,
        q,
        rb,
        rbc,
        rp,
        rt,
        rtc,
        ruby,
        s,
        samp,
        script,
        select,
        small,
        span,
        strike,
        strong,
        style,
        sub,
        sup,
        table,
        tbody,
        td,
        textarea,
        tfoot,
        th,
        thead,
        title,
        tr,
        tt,
        u,
        ul,
        var,
        xmp,
        nextid
    }
}

