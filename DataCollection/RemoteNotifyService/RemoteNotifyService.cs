using RemoteNotify.EventLogCollection;
using System.ServiceProcess;
using System.Threading;

namespace RemoteNotify
{
    public partial class RemoteNotifyService : ServiceBase
    {
        //Used to debug the service
        public void OnDebug()
        {
            OnStart(null);
        }
        
        public RemoteNotifyService()
        {
            InitializeComponent();
        }

        Thread NetworkLoggingThread;
        Thread WinRMEventCollectorThread;
        //Thread SystemLoggingThread;

        protected override void OnStart(string[] args)
        {
            //NetworkLogs NetworkCollector = new NetworkLogs();
            //NetworkLoggingThread = new Thread(NetworkCollector.CollectNetworkLogs);

            WinRMEvents WinRMCollector = new WinRMEvents();
            WinRMEventCollectorThread = new Thread(WinRMCollector.CollectWinRMEvents);
            //SystemLoggingThread = new Thread(CollectSystemLogs);

            WinRMEventCollectorThread.Start();
            //NetworkLoggingThread.Start();
            //SystemLoggingThread.Start();
        }

        protected override void OnStop()
        {
            //NetworkLoggingThread.Interrupt();
            //NetworkLoggingThread.Join();

            WinRMEventCollectorThread.Interrupt();
            WinRMEventCollectorThread.Join();

            //SystemLoggingThread.Interrupt();
            //SystemLoggingThread.Join();
        }

        

        
    }
}
