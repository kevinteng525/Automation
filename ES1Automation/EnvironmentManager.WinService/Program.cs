using System.ServiceProcess;

namespace EnvironmentManager.WinService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[] 
			{ 
				new EnvrionmentMgrService() 
			};

            ServiceBase.Run(ServicesToRun);
        }
    }
}
