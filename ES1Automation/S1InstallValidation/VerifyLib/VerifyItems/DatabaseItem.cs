using System;
using System.Globalization;
using System.Xml.Linq;




namespace VerifyLib.VerifyItems
{
    public class DatabaseItem : VerifyItem
    {
        private const string ConnectionString = "server={0};database={1};User Id={2};Password={3};Connect Timeout=3";

        public string ServerName;

        public string DbName;

        public string SqlQuery;

        protected string Uid;

        protected string Pwd;

        public DatabaseItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            ServerName = GetAttributeValue("server");

            DbName = GetAttributeValue("dbName");


            Uid = GetAttributeValue("uid");

            Pwd = GetAttributeValue("pwd");

            SqlQuery = GetAttributeValue("sqlQuery");
        }

        protected override void Verify()
        {
            try
            {
                var sqlServer = new Common.SQL.SQLServer(string.Format(ConnectionString, ServerName, DbName, Uid, Pwd));

                ActualValue = sqlServer.ExecuteSqlReturnString(SqlQuery);

                VerifyResult = ActualValue == CheckValue ? VerifyResult.Pass : VerifyResult.Failed;
            }
            catch(Exception ex)
            {
                VerifyResult = VerifyResult.Failed;
                Information = ex.Message;
            }
        }
    }
}
