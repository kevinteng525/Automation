using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;

using Microsoft.SharePoint.Client;

namespace ES1OnlineTestDataImport
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigParser configParser = new ConfigParser(System.AppDomain.CurrentDomain.BaseDirectory + @"\Config.xml");
            StreamWriter log = new StreamWriter("TestData.log");
            BaseAuthentication authentication = null;
            log.AutoFlush = true;
            log.WriteLine("Start: " + DateTime.Now.ToString());
            log.WriteLine("Item ID\tFile Name\tList Title\tFile Extension\tFile Size\tCreated Date\tModified Date");
            try
            {
                User currentUser = new User()
                {
                    UserName = configParser.GetNodeAttibuteValue(@"//User", "Name", "Password").GetKey(0).Trim(),
                    Password = configParser.GetNodeAttibuteValue(@"//User", "Name", "Password").GetValues(0)[0].Trim()
                };

                authentication = Authenticate(configParser, currentUser);

                FileUploadImp fileUploader = new FileUploadImp(authentication.clientContext, System.AppDomain.CurrentDomain.BaseDirectory+@"\files", configParser, log);
                fileUploader.CreateOrOpenWebs();
                log.WriteLine("Terminate: " + DateTime.Now.ToString());
                Console.WriteLine("All Done! Have fun!");
                Console.WriteLine("Press any key to continue..."); 
                Console.Read();
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.Message + "--->" + ex.StackTrace);
                Console.WriteLine(ex.Message);
                Console.Read();
            }
            finally
            {
                log.Close();
                authentication.Close();
            }
        }

        private static BaseAuthentication Authenticate(ConfigParser configParser, User currentUser)
        {
            try
            {
                BaseAuthentication authentication = CreateAuthentication(configParser, currentUser);
                authentication.Authenticate();
                Web web = authentication.clientContext.Web;
                authentication.clientContext.Load(web);
                authentication.clientContext.ExecuteQuery();
                return authentication;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static BaseAuthentication CreateAuthentication(ConfigParser configParser, User currentUser)
        {
            string targetWebAppLocation = configParser.GetSingleNodeAttributeValue("WebApp", "URL");
            NameValueCollection metaHandlerInfo = configParser.GetNodeAttibuteValue(@"//Identity", "Type", "Model");
            Type t = Type.GetType(metaHandlerInfo.GetValues(0)[0].Trim());
            BaseAuthentication authentication = System.Activator.CreateInstance(t, new object[] { currentUser, targetWebAppLocation }) as BaseAuthentication;
            return authentication;
        }
    }
}
