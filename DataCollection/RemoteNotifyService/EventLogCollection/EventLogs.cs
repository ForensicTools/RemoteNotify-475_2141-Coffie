using System.Diagnostics;
using System.Threading;

namespace RemoteNotify
{
    class EventLogs
    {
        //Collects events from the system log
        public void CollectEventLogs()
        {
            while (true)
            {
                try
                {
                    //Have it run every second
                    Thread.Sleep(1000);
//
//                    //Create a string to contain the powershell script
//                    //script will write events to log file
//                    String script = Resources.CollectSystemLogsPS;
//
//                    //Create Powershell runspace and run it
//                    Runspace runspace = RunspaceFactory.CreateRunspace();
//                    runspace.Open();
//                    //Create a pipeline and feed it the script text
//                    Pipeline pipeline = runspace.CreatePipeline();
//                    pipeline.Commands.AddScript(script);
//                    //Convert output to strings
//                    pipeline.Commands.Add("Out-String");
//                    //Execute command
//                    Collection<PSObject> results = pipeline.Invoke();
//                    //Close the runspace
//                    runspace.Close();

					//Logs needed for WinRM
					EventLog WinRMEventLog = new EventLog("Microsoft-Windows-WinRM/Operational");
					//WinRMEventLog.Entries now contains all of the WinRM Logs

					//Logs needed for psexec
					EventLog PSExecSecurityLog = new EventLog("Security");
					EventLog PSExecSystemLog = new EventLog("System");




                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
            }
        }
    }
}
