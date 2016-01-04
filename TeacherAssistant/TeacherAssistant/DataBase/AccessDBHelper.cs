
using ADOX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading.Tasks;
using TeacherAssistant.Model;

namespace TeacherAssistant.DataBase
{
    public static class AccessDBHelper
    {
        //数据库对象
        private static ADOX.Catalog catalog = new Catalog();
        private static OleDbConnection connection;
        //创建数据库文件
        public static void CreateDB(string path)
        {
            try
            {
                string connectstr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Jet OLEDB:Engine Type = 5", path);
                catalog.Create(connectstr);
                Debug.WriteLine("DataBase created");
            }
            catch (Exception create)
            {
                Debug.WriteLine(create.Message);
                Debug.WriteLine("!!!Databese can not be created.!!!");

            }
        }
        public static void ConnectDB(string path)
        {

            //连接字符串
            string connstr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", path);
            connection = new OleDbConnection(connstr);
            Debug.WriteLine("连接");
            connection.Open();

            //ADODB.Connection cn = new ADODB.Connection();
            ////打开连接
            //cn.Open(connstr, null, null, -1);
            //catalog.ActiveConnection = cn;

            //ADOX.Table table = new ADOX.Table();
            //table.Name = "ClassTable";
            //ADOX.Column column = new ADOX.Column();
            //column.ParentCatalog = catalog;
            //column.Name = "RecordId";
            //column.Type = DataTypeEnum.adInteger;
            //column.DefinedSize = 9;
            //column.Properties["AutoIncrement"].Value = true;
            //table.Columns.Append(column, DataTypeEnum.adInteger, 9);
            //table.Keys.Append("FirstTablePrimaryKey", KeyTypeEnum.adKeyPrimary, column, null, null);
            //table.Columns.Append("CustomerName", DataTypeEnum.adVarWChar, 50);
            //table.Columns.Append("Age", DataTypeEnum.adInteger, 9);
            //table.Columns.Append("Birthday", DataTypeEnum.adDate, 0);
            //catalog.Tables.Append(table);
            //cn.Close();
        }
        public static void CloseConnectDB()
        {
            connection.Close();
            connection.Dispose();
        }
        public static void CreateTable(string sql, string path)
        {
            string connstr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", path);
            connection = new OleDbConnection(connstr);
            OleDbCommand cmd = new OleDbCommand(sql, connection);
            try
            {
                connection.Open();
                int affect = cmd.ExecuteNonQuery();
                Debug.WriteLine("受影响的行数{0}", affect);
            }
            catch (Exception exec)
            {

                Debug.WriteLine(exec.Message);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //  public static readonly string conn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=d:\\test.mdb;Jet OLEDB:Engine Type=5";// + HttpContext.Current.Request.PhysicalApplicationPath + System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        // 用于缓存参数的HASH表
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        ///  给定连接的数据库用假设参数执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 用现有的数据库连接执行一个sql命令（不返回数据集）
        /// </summary>
        /// <remarks>
        ///举例:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个现有的数据库连接</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery(string sql, string path)
        {
            if (connection.State != ConnectionState.Open)
                ConnectDB(path);
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            int val = 0;
            //PrepareCommand(cmd, connection, null, sql, commandParameters);
            try
            {
                val = (int)cmd.ExecuteScalar();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            cmd.Parameters.Clear();
            //CloseConnectDB();
            return val;
        }
        /// <summary>
        ///使用现有的SQL事务执行一个sql命令（不返回数据集）
        /// </summary>
        /// <remarks>
        ///举例:  
        ///  int result = ExecuteNonQuery(trans, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个现有的事务</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery(OleDbTransaction trans, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// 用执行的数据库连接执行一个返回数据集的sql命令
        /// </summary>
        /// <remarks>
        /// 举例:  
        ///  OleDbDataReader r = ExecuteReader(connString, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>包含结果的读取器</returns>
        public static OleDbDataReader ExecuteReader(string sql, string filepath)
        {
            string connstr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", filepath);
            connection = new OleDbConnection(connstr);
            //创建一个SqlCommand对象
            OleDbCommand cmd = new OleDbCommand(sql, connection);
            //创建一个SqlConnection对象
            //connection = new OleDbConnection(connectionString);
            //在这里我们用一个try/catch结构执行sql文本命令/存储过程，因为如果这个方法产生一个异常我们要关闭连接，因为没有读取器存在，
            //因此commandBehaviour.CloseConnection 就不会执行
            try
            {
                connection.Open();

                //调用 PrepareCommand 方法，对 SqlCommand 对象设置参数
                // PrepareCommand(cmd, connection, null, cmdText, commandParameters);
                //调用 SqlCommand  的 ExecuteReader 方法
                //OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                OleDbDataReader reader = cmd.ExecuteReader();
                //清除参数
                cmd.Parameters.Clear();
                return reader;
            }
            catch (Exception e)
            {
                //关闭连接，抛出异常
                CloseConnectDB();
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        /// <summary>
        /// 返回一个DataSet数据集
        /// </summary>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>包含结果的数据集</returns>
        public static DataSet ExecuteDataSet(string connectionString, string cmdText, params OleDbParameter[] commandParameters)
        {
            //创建一个SqlCommand对象，并对其进行初始化
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdText, commandParameters);
                //创建SqlDataAdapter对象以及DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    //填充ds
                    da.Fill(ds);
                    // 清除cmd的参数集合 
                    cmd.Parameters.Clear();
                    //返回ds
                    return ds;
                }
                catch
                {
                    //关闭连接，抛出异常
                    conn.Close();
                    throw;
                }
            }
        }
        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <remarks>
        ///例如:  
        ///  Object obj = ExecuteScalar(connString, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        ///<param name="connectionString">一个有效的连接字符串</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns>
        public static object ExecuteScalar(string connectionString, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 用指定的数据库连接执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <remarks>
        /// 例如:  
        ///  Object obj = ExecuteScalar(connString, "PublishOrders", new OleDbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个存在的数据库连接</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns>
        public static object ExecuteScalar(OleDbConnection connection, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, connection, null, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// 将参数集合添加到缓存
        /// </summary>
        /// <param name="cacheKey">添加到缓存的变量</param>
        /// <param name="cmdParms">一个将要添加到缓存的sql参数集合</param>
        public static void CacheParameters(string cacheKey, params OleDbParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }
        /// <summary>
        /// 找回缓存参数集合
        /// </summary>
        /// <param name="cacheKey">用于找回参数的关键字</param>
        /// <returns>缓存的参数集合</returns>
        public static OleDbParameter[] GetCachedParameters(string cacheKey)
        {
            OleDbParameter[] cachedParms = (OleDbParameter[])parmCache[cacheKey];
            if (cachedParms == null)
                return null;
            OleDbParameter[] clonedParms = new OleDbParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms = (OleDbParameter[])((ICloneable)cachedParms).Clone();
            return clonedParms;
        }
        /// <summary>
        /// 准备执行一个命令
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="conn">Sql连接</param>
        /// <param name="trans">Sql事务</param>
        /// <param name="cmdText">命令文本,例如：Select * from Products</param>
        /// <param name="cmdParms">执行命令的参数</param>
        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, string cmdText, OleDbParameter[] cmdParms)
        {
            //判断连接的状态。如果是关闭状态，则打开
            if (conn.State != ConnectionState.Open)
                conn.Open();
            //cmd属性赋值
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //是否需要用到事务处理
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            //添加cmd需要的存储过程参数
            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        /// <summary>
        /// 事务，一直执行多次插入
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="filepath"></param>
        public static bool Transaction(string[] cmdText, string filepath)
        {

            //连接字符串
            string connstr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", filepath);
            connection = new OleDbConnection(connstr);
            connection.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = connection;
            cmd.Transaction = connection.BeginTransaction();
            try
            {
                for (int i = 0; i < cmdText.Length; i++)
                {
                    string strsql = cmdText[i].Trim();
                    if (strsql.Length > 1)
                    {
                        cmd.CommandText = strsql;
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd.Transaction.Commit();
                connection.Close();
                return true;
            }
            catch (Exception x)
            {
                cmd.Transaction.Rollback();

                connection.Close();
                Debug.WriteLine(x.Message);
                return false;
            }
        }


        public static List<TeachClassStu> GetStuList(string courseclassnum)
        {
            //AccessDBHelper.ConnectDB(App.Databasefilepath);
            List<TeachClassStu> studatalist = new List<TeachClassStu>();
            string sql = "select * from " + courseclassnum;
            OleDbDataReader reader = AccessDBHelper.ExecuteReader(sql, App.Databasefilepath);
            while (reader.Read())
            {
                TeachClassStu tcsa = new TeachClassStu();
                tcsa.StuNum = reader["stunum"].ToString();
                tcsa.StuName = reader["stuname"].ToString();
                tcsa.Sex = reader["sex"].ToString();
                tcsa.Subject = reader["subject"].ToString();
                tcsa.ClassNum = reader["classnum"].ToString();
                tcsa.ClassState = reader["classstate"].ToString();
                tcsa.ClassType = reader["classtype"].ToString();
                tcsa.Num = Convert.ToInt32(reader["num"].ToString());
                studatalist.Add(tcsa);
            }
            reader.Close();
            AccessDBHelper.CloseConnectDB();
            return studatalist;
        }
    }
}
