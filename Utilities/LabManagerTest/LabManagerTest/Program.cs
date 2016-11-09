using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using LabManagerTest.LabManager;
using System.Configuration;


namespace LabManagerTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string configName = ConfigurationManager.AppSettings["configName"];
            string configLibName = ConfigurationManager.AppSettings["configLibName"];

            try
            {
                LabManagerSOAPinterfaceSoapClient client = new LabManager.LabManagerSOAPinterfaceSoapClient();

                #region authentication part
                AuthenticationHeader authenticationHeader = new AuthenticationHeader();
                authenticationHeader.username = ConfigurationManager.AppSettings["username"];
                authenticationHeader.password = ConfigurationManager.AppSettings["password"];
                authenticationHeader.organizationname = ConfigurationManager.AppSettings["organizationname"];
                authenticationHeader.workspacename = ConfigurationManager.AppSettings["workspacename"];
                #endregion

                LabManager.Configuration myConfig = client.GetSingleConfigurationByName(authenticationHeader, configName);
                LabManager.Configuration myConfigLib = client.GetSingleConfigurationByName(authenticationHeader, configLibName);

                Console.WriteLine("Name: " + myConfig.name);
                Console.WriteLine("ID = " + myConfig.id.ToString());
                Console.WriteLine("Description = " + myConfig.description);
                Console.WriteLine("isPublic = " + myConfig.isPublic.ToString());
                Console.WriteLine("isDeployed = " + myConfig.isDeployed.ToString());
                Console.WriteLine("fenceMode = " + myConfig.fenceMode.ToString());
                Console.WriteLine("type = " + myConfig.type.ToString());
                Console.WriteLine("owner = " + myConfig.owner);
                Console.WriteLine("dateCreated = " + myConfig.dateCreated.ToString());
                Console.WriteLine("Deployed = " + myConfig.isDeployed);

                if (myConfig.isDeployed)
                {
                    client.ConfigurationUndeploy(authenticationHeader, myConfig.id);
                }
                client.ConfigurationDelete(authenticationHeader, myConfig.id);
                client.ConfigurationCheckout(authenticationHeader, myConfigLib.id, configName);
                //Assgin the new checked out config to myconfig.
                myConfig = client.GetSingleConfigurationByName(authenticationHeader, configName);
                client.ConfigurationDeploy(authenticationHeader, myConfig.id, false, 1);
                               
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            
        }
    }
}
