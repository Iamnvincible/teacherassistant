using ADOX;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherAssistant.DataBase
{
    public static class AccessDBHelper
    {
        //数据库对象
        private static ADOX.Catalog catalog = new Catalog();
        const string connectstrbegin = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
        const string connectstrend = ";Jet OLEDB:Engine Type=5";
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
            }
        }
        public static void ConnectDB(string path)
        {

            //连接字符串
            string connstr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", path);
            connection = new OleDbConnection(connstr);
            //connection.Open();

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
        public static void CreateTable(string sql,string path)
        {
            string connstr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", path);
            connection = new OleDbConnection(connstr);
            OleDbCommand cmd = new OleDbCommand(sql, connection);
            try
            {

                connection.Open();
                int affect= cmd.ExecuteNonQuery();
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
    }
}
