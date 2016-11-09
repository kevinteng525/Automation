using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Common.DBCommon
{
    /// <summary>
    /// A common class for MS SQL Server
    /// </summary>
    public class MSSqlServer
    {
        private readonly SqlConnection sqlConnection;

        private SqlCommand sqlCommand;

        private SqlDataAdapter sqlDataAdapter;

        private SqlDataReader sqlDataReader;

        public MSSqlServer(string connectionString)
        {
            try
            {
                sqlConnection = new SqlConnection  (connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        #region SqlString

        public void ExecuteSql(string sqlString)
        {
            try
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public DataSet ExecuteSqlReturnDataSet(string sqlString)
        {
            try
            {
                sqlConnection.Open();
                sqlDataAdapter = new SqlDataAdapter(sqlString, sqlConnection);
                var ds = new DataSet();
                sqlDataAdapter.Fill(ds);
                sqlConnection.Close();
                return ds;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public string ExecuteSqlReturnString(String sqlString)
        {
            try
            {
                string result = string.Empty;
                sqlConnection.Open();
                sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    result = sqlDataReader[0].ToString();

                }
                sqlDataReader.Close();
                return result;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public byte[] ExecuteSqlReturnByteArray(String sqlString)
        {
            try
            {
                byte[] result =  null;
                sqlConnection.Open();
                sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    if (sqlDataReader[0] != DBNull.Value)
                    {
                        result = (byte[]) sqlDataReader[0];
                    }
                }
                sqlDataReader.Close();
                return result;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool ExecuteSQLJodgeIsExist(String sqlString)
        {
            try
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    return true;
                }
                return false;
            }
            finally
            {
                sqlDataReader.Close();
                sqlConnection.Close();
            }
        }

        public List<string> ExecuteSqlReturnList(String sqlString)
        {
            try
            {
                List<string> result = new List<string>();
                sqlConnection.Open();
                sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
                while(sqlDataReader.Read())
                {
                    result.Add(sqlDataReader[0].ToString());
                }
                sqlDataReader.Close();
                return result;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        #endregion

        #region sqlParameters

        private void CommandAddParams(SqlParameter[] sqlParameters)
        {
            foreach (SqlParameter sqlParameter in sqlParameters)
            {
                sqlCommand.Parameters.Add(sqlParameter);
            }
        }

        private void DataAdapterAddParams(SqlParameter[] sqlParameters)
        {
            foreach (SqlParameter sqlParameter in sqlParameters)
            {
                sqlDataAdapter.SelectCommand.Parameters.Add(sqlParameter);
            }
        }

        public void ExecuteSql(string sqlString, SqlParameter[] sqlParameters)
        {
            try
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand(sqlString, sqlConnection);
                CommandAddParams(sqlParameters);
                sqlCommand.ExecuteNonQuery();
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool ExecuteSQLJodgeIsExist(string sqlString, SqlParameter[] sqlParameters)
        {
            try
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand(sqlString, sqlConnection);
                CommandAddParams(sqlParameters);
                sqlDataReader =  sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    return true;
                }
                return false;
            }
            finally
            {
                sqlDataReader.Close();
                sqlConnection.Close();
            }
        }

        public DataSet ExecuteSqlReturnDataSet(string sqlString, SqlParameter[] sqlParameter)
        {
            try
            {
                sqlConnection.Open();
                sqlDataAdapter = new SqlDataAdapter(sqlString, sqlConnection);
                DataAdapterAddParams(sqlParameter);
                var ds = new DataSet();
                sqlDataAdapter.Fill(ds);
                sqlConnection.Close();
                return ds;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        
        #endregion
    }
}
