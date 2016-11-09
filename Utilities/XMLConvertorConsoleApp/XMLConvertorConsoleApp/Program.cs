using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLConvertorConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int parameterNum = 2;

            if (args.Length != parameterNum)
            {
                Console.WriteLine("Please input the valid source xml file path and xslt file path as parameters.");
                Console.WriteLine(@"Example: XMLConvertorConsoleApp.exe C:\TestingResult.xml C:\TestingResult.xsl");
                return;
            }

            try
            {
                System.Xml.Xsl.XslCompiledTransform xslt = new System.Xml.Xsl.XslCompiledTransform();
                string xmlSourcePath = args[0];
                string xsltFilePath = args[1];
                xslt.Load(xsltFilePath);
                xslt.Transform(xmlSourcePath, "TestResult.html");
            }
            catch (Exception e)
            {
                
                Console.WriteLine("The application encounter an error, refer the following detail information:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
