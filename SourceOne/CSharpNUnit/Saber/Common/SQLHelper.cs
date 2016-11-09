using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;




namespace Saber.Common
{
    public class SQLHelper
    {
        public void RunSQLScript(String DataSource, String fileSQL, String sqlConString)
        {
            //string sqlConnectionString = "Data Source=Debug02;Initial Catalog=AdventureWorks;Integrated Security=True";
            //string sqlConnectionString = "data source=Debug02; user id=sa; password=emcsiax@QA";
            string sqlConnectionString = sqlConString;
            Console.WriteLine("SQL connection string = " + sqlConnectionString);
            SqlConnection conn = new SqlConnection();
            try
            {                
                conn.ConnectionString = sqlConnectionString;

                FileInfo file = new FileInfo(fileSQL);

                string script = file.OpenText().ReadToEnd();

                Server server = new Server(new ServerConnection(conn));

                server.ConnectionContext.ExecuteNonQuery(script);

                Console.WriteLine("Successfully runned the SQL script, name = " + fileSQL);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: RunSQLScript() failed with error :" + e);
                throw new Exception();                
            }
            finally
            {
                conn.Close();

            }
        }
    }
}
