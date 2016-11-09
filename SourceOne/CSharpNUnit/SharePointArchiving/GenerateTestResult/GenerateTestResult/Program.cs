using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace GenerateTestResult
{
    public class Program
    {
        public static int Passed = 0;
        public static int Failed = 0;
        public static int Total = 0;

        static void Main(string[] args)
        {
            GenerateTestResult(args[0]);
        }

        public static void GenerateTestResult(string path)
        {
            String testResultSum = "";
            foreach (string file in Directory.GetFiles(path, "*.trx"))
            {
                testResultSum = testResultSum + GetResult(file);
            }
            Console.Write(testResultSum);
            Total = Passed + Failed;
            String contents = @"<Results Total = '" + Total.ToString() + "' Passed = '" + Passed.ToString() + "' Failed = '" + Failed.ToString()
                + "'>" + testResultSum + @"</Results>";
            File.WriteAllText("TestResult.xml", contents);
        }

        public static String GetResult(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList resultNodes = doc.GetElementsByTagName("Results");
            XmlElement result = (XmlElement)resultNodes[0];
            XmlElement unitestResult = (XmlElement)result.ChildNodes[0];
            string outcome = unitestResult.GetAttribute("outcome");
            if (outcome=="Passed")
                Passed++;
            else
                Failed++;
            return result.InnerXml;
        }

        
    }
}
