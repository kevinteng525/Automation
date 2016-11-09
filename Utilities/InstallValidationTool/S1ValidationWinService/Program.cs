using System.ServiceProcess;

namespace S1ValidationWinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] 
                                            {
                                               new S1ValidationService() 
                                            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}
