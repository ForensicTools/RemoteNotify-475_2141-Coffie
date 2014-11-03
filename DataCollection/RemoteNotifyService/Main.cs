using System.ServiceProcess;

namespace RemoteNotify
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            RemoteNotifyService myService = new RemoteNotifyService();
            myService.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RemoteNotifyService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
