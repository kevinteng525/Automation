using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

using Saber.TestEnvironment;

namespace Saber.TestData.EWS
{
    public class MDBUtil
    {
        private string MDBPath;
        private string UsersTableName = "UserNames";
        private string MessagesTableName = "Messages";
        private string Domain = "";
        public DataTable MessagesTable;
        public DataTable UserNamesTable;

        public bool isMessageSuccess = true;
        public bool isUserNameSuccess = true;
        private int userRowCount = 0;
        private int messageRowCount = 0;

        public int From = 1;
        public int To = 2;
        public int CC = 3;
        public int BCC = 4;
        public int Subject = 5;
        public int Body = 6;
        public int Attachments = 7;

        public MDBUtil(string mdbPath)
        {
            Domain = TestEnvironmentHelper.DomainName;
            MDBPath = mdbPath;
            MessagesTable = ReadAllData(MessagesTableName, ref isMessageSuccess);
            UserNamesTable = ReadAllData(UsersTableName, ref isUserNameSuccess);
        }

        public int MessageRowCount
        {
            get
            {
                if (isMessageSuccess)
                {
                    return MessagesTable.Rows.Count;
                }
                return 0;
            }
            set
            {
                messageRowCount = value;
            }
        }

        public int UserRowCount
        {
            get
            {
                if (isUserNameSuccess)
                {
                    return UserNamesTable.Rows.Count;
                }
                return 0;
            }
            set
            {
                userRowCount = value;
            }
        }

        //Find corresponding Email Address from UserNames table by UserName
        public string GetEmailAddress(string userNames)
        {
            if (string.IsNullOrEmpty(userNames))
                return null;

            string emailAddress = "";
            string tempAddress;
            string trimmedName;
            if (userNames.Contains(";"))
            {
                string[] userList= userNames.Split(new char[] { ';' });
                foreach (string userName in userList)
                {
                    trimmedName = userName.Trim();
                    DataRow[] filterRows = UserNamesTable.Select("UserName = '" + trimmedName + "'");
                    tempAddress = filterRows[0][4].ToString();
                    if(tempAddress.Contains("@"))
                    {
                        emailAddress = emailAddress + tempAddress + ";";
                    }
                    else
                    {
                        emailAddress = emailAddress + tempAddress + "@" + Domain + ";";
                    }
                    
                }
            }
            else
            {
                trimmedName = userNames.Trim();
                DataRow[] filterRows = UserNamesTable.Select("UserName = '" + trimmedName + "'");
                tempAddress = filterRows[0][4].ToString();
                if(tempAddress.Contains("@"))
                {
                    emailAddress = tempAddress;
                }
                else
                {
                    emailAddress = tempAddress + "@" + Domain;
                }
            }
            emailAddress = emailAddress.TrimEnd(new char[] { ';' });
            return emailAddress;
        }

        private DataTable ReadAllData(string tableName, ref bool success)   
        {       
            DataTable dt = new DataTable();       
            try
            {   
                DataRow dr;    
      
                string strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="   + MDBPath + ";";   
                OleDbConnection odcConnection = new OleDbConnection(strConn);    
                odcConnection.Open();    

                OleDbCommand odCommand = odcConnection.CreateCommand();    
                odCommand.CommandText = "select * from " + tableName;    
   
                OleDbDataReader odrReader = odCommand.ExecuteReader();    

                int size = odrReader.FieldCount;   
                for (int i = 0; i < size; i++)   
                {       
                    DataColumn dc;       
                    dc = new DataColumn(odrReader.GetName(i));       
                    dt.Columns.Add(dc);   
                }   
                while (odrReader.Read())   
                {       
                    dr = dt.NewRow();       
                    for (int i = 0; i < size; i++)       
                    {   
                        dr[odrReader.GetName(i)] =   odrReader[odrReader.GetName(i)].ToString();       
                    }       
                    dt.Rows.Add(dr);   
                }   

                odrReader.Close();   
                odcConnection.Close();   
                success = true;   
                return dt;       
            }       
            catch       
            {   
                success = false;   
                return dt;       
            }   
        } 
    }
}
