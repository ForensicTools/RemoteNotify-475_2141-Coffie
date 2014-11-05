using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteNotify.EventLogCollection
{
    class WinRMEvents
    {
        private String RemoteNotifyLogsPath = "C:\\RemoteNotify\\Logs";
        private String LogName = "WinRMEvents.csv";
        public void CollectWinRMEvents()
        {
            //Check if log path is created
            if (!(Directory.Exists(RemoteNotifyLogsPath)))
            {
                Directory.CreateDirectory(RemoteNotifyLogsPath);
            }
            
            //Check if log is created
            if(!(File.Exists(RemoteNotifyLogsPath + "\\" + LogName)))
            {
                //create CSV File and add Header column names
                using (StreamWriter writer = File.CreateText(RemoteNotifyLogsPath + "\\" + LogName))
                {
                    writer.WriteLine("TimeGenerated,TimeLogged,TimeCollectedByRemoteNotify,InstanceID,MachineName,Message,SourceApplication,UserResponsible,State,OwningPID,OwningProcessName,ConnectionGuid");
                }
            }

            while (true)
            {
                try
                {
                    //Have it run every second
                    Thread.Sleep(1000);

                    //Logs needed for WinRM
                    EventLog WinRMEventLog = new EventLog("Microsoft-Windows-WinRM/Operational");
                    //WinRMEventLog.Entries now contains all of the WinRM Logs

                    //Check if it's an event we're interested in
                    using (StreamWriter writer = new StreamWriter("C:\\RemoteNotify\\test.txt", true))
                    {
                        writer.WriteLine("Program Ran");
                        foreach (EventLogEntry LogEntry in WinRMEventLog.Entries)
                        {
                            writer.WriteLine(LogEntry.InstanceId.ToString());
                        }
                    }

                    foreach(EventLogEntry LogEntry in WinRMEventLog.Entries)
                    {
                        //Deserialize data
                        Hashtable EventData = null;
                        MemoryStream EventDataStream = new MemoryStream(LogEntry.Data);
                        IFormatter EventDataFormatter = new BinaryFormatter();
                        EventData = (Hashtable) EventDataFormatter.Deserialize(EventDataStream);
                        EventDataStream.Close();


                        using (StreamWriter writer = new StreamWriter("C:\\test.txt", true))
                        {
                            writer.WriteLine(EventData.Keys.ToString() + "---");
                        }
                        //Write to log if originating by Winrm
                        //using (StreamWriter writer = new StreamWriter(RemoteNotifyLogsPath + "\\" + LogName, true))
                        //{
                        //    writer.WriteLine(LogEntry.TimeGenerated.ToString(); + "," +
                        //            DateTime.Now.ToString() + "," +
                        //            LogEntry.TimeWritten.ToString() + "," +
                        //            LogEntry.InstanceId.ToString() + "," +
                        //            LogEntry.MachineName.ToString() + "," +
                        //            LogEntry.Message.ToString() + "," +
                        //            LogEntry.Source.ToString() + "," +
                        //            LogEntry.UserName.ToString() + "," +
                        //             + "," +
                        //            Process.GetProcessById(checked((int)Connection.Connection.owningPid)).ProcessName + "," +
                        //            Connection.ConnectionGuid.ToString()
                        //            );
                        //}
                    }

                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
            }
        }
    }
}
