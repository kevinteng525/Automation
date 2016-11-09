using System;
using System.Configuration;
using System.ServiceProcess;
using S1ValidationWinService.Maintains;
using S1ValidationWinService.NetTransmission;

namespace S1ValidationWinService
{
    public partial class S1ValidationService : ServiceBase
    {
        private Server server;

        public S1ValidationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLogger.LogEvent("=========== Validation Service Begin to Start ===========");

            try
            {
                string serverName = ConfigurationManager.AppSettings["ServerName"];
                string ipAdderss = ConfigurationManager.AppSettings["ListenAddress"];

                int port;
                int.TryParse(ConfigurationManager.AppSettings["TcpPort"], out port);

                server = ServerFactory.CreateServer(serverName, ipAdderss, port);
                server.StartListener();
            }
            catch (Exception ex)
            {
                EventLogger.LogError(ex.Message);

                throw;
            }

            EventLogger.LogEvent("=========== Validation Service Has Started ===========");
        }

        protected override void OnStop()
        {
            EventLogger.LogEvent("=========== Validation Service Begin to Stop ===========");

            server.StopListener();

            EventLogger.LogEvent("=========== Validation Service Has Stopped ===========");
        }
    }
}
