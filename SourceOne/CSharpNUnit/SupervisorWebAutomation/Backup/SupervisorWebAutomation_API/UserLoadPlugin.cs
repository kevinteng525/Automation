using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.LoadTesting;


namespace SupervisorWebAutomation_API
{

    public class UserLoadPlugin : ILoadTestPlugin
    {
        string m_fileName = @"C:\SaberAgent\Config\userload.txt";

        LoadTest m_loadTest;

        public void Initialize(LoadTest loadTest)
        {
            m_loadTest = loadTest;

            m_loadTest.Heartbeat += new System.EventHandler<HeartbeatEventArgs>(m_loadTest_Heartbeat);
        }

        void m_loadTest_Heartbeat(object sender, HeartbeatEventArgs e)
        {
            int load = GetUserLoadFromFile();

            if (load != -1)
            {
                m_loadTest.Scenarios[0].CurrentLoad = load;
            }           
        }       

        private int GetUserLoadFromFile()
        {
            int newLoad = -1;

            try
            {
                using (StreamReader streamReader = new StreamReader(m_fileName))
                {
                    string load = streamReader.ReadToEnd();

                    try
                    {
                        if (!string.IsNullOrEmpty(load))
                        {
                            newLoad = int.Parse(load);
                        }
                    }
                    catch (FormatException)
                    {
                        //ignore
                    }
                }

            }

            catch (IOException)
            {
                //ignore
            }

            return newLoad;
        }

    }
}



