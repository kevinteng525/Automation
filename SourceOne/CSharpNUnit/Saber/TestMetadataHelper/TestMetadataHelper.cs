using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

using Saber.Common;

namespace Saber.TestMetadata
{
    public class TestMetadataHelper
    {
        private static readonly String environmentConfigFile = @"C:\SaberAgent\Config\Environment.xml";
        private String testcaseConfigFile = "";

        public TestMetadataHelper(String configFilePath)
        {
            this.testcaseConfigFile = Path.Combine(AssemblyHelper.GetAssemblePath(), "TestMetadata", configFilePath);
        }

        private String GetParameterValueByPath(XElement element, String [] path )
        {
            XElement temp = element;
            foreach(String name in path)
            {
                if(temp.Element(name)!=null)
                {
                    temp = temp.Element(name);
                }
                else
                {
                    //throw new Exception("The node doesn't exist!");
                    return null;
                }
            }
            return temp.Value;
        }

        public String GetParameter(params String [] parameterPath)
        {
            XDocument environment = XDocument.Load(environmentConfigFile);
            XDocument testcase = XDocument.Load(testcaseConfigFile);
            XElement environmentConfigs = environment.Root.Element("s1configs");
            XElement testcaseConfigs = testcase.Root;
            String value = GetParameterValueByPath(testcaseConfigs, parameterPath);
            if (null != value)
            {
                return value;
            }
            else
            {
                value = GetParameterValueByPath(environmentConfigs, parameterPath);
                if (null != value)
                {
                    return value;
                }
                else
                {
                    String formattedPath = "";
                    foreach(String name in parameterPath)
                    {
                        formattedPath += name + "->";
                    }
                    throw new Exception("Cann't find the value in the test case config file and the environment config file. Please check the path! " + formattedPath);
                }
            }
        }    
    }
}
