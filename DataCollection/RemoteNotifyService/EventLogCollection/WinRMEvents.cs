using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
        private String WindowsLogDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + "Windows\\Sysnative\\winevt\\Logs";

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

                    //Check if windows log exists
                    if (File.Exists(WindowsLogDirectory + "\\" + "Microsoft-Windows-WinRM%4Operational.evtx"))
                    {
                        String LastRecordedEventTime = LastLogCollectedTime();
                        EventLogQuery WinRMEventLogQuery;
                        
                        if (LastRecordedEventTime == "")
                        {
                            WinRMEventLogQuery = new EventLogQuery(WindowsLogDirectory + "\\" + "Microsoft-Windows-WinRM%4Operational.evtx", PathType.FilePath);
                            bool test = File.Exists(WindowsLogDirectory + "\\" + "Microsoft-Windows-WinRM%4Operational.evtx");
                        }
                        else
                        {
                            // Only return events since last recorded event
                            String QueryString = "*[System[TimeCreated[@SystemTime >= \'" + LastRecordedEventTime + "\']]]";
                            WinRMEventLogQuery = new EventLogQuery(WindowsLogDirectory + "\\" + "Microsoft-Windows-WinRM%4Operational.evtx", PathType.FilePath, QueryString);
                            bool test = File.Exists(WindowsLogDirectory + "\\" + "Microsoft-Windows-WinRM%4Operational.evtx");
                        }

                        EventLogReader logReader;
                        try
                        {
                            // Query the log and create a stream of selected events
                            logReader = new EventLogReader(WinRMEventLogQuery);
                        }
                        catch (EventLogNotFoundException)
                        {
                            return; //Failed to query the WinRM log!
                        }

                        for (EventRecord eventInstance = logReader.ReadEvent(); eventInstance != null; eventInstance = logReader.ReadEvent())
                        {
                            String eventXml = eventInstance.ToXml();
                            Console.WriteLine(eventXml);
                            using (StreamWriter writer = new StreamWriter(RemoteNotifyLogsPath + "\\" + LogName, true))
                            {
                                writer.WriteLine(eventXml);
                            }
                        }

                            //Write to log if originating by Winrm
                            //using (StreamWriter writer = new StreamWriter(RemoteNotifyLogsPath + "\\" + LogName, true))
                            //{
                            //    writer.WriteLine(LogEntry.TimeGenerated.ToString() + "," +
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
        private String LastLogCollectedTime()
        {
            //Check if log path is created
            if (!(Directory.Exists(RemoteNotifyLogsPath)))
            {
                // Log Directory doesn't exist yet
                return "";
            }

            //Check if log is created
            if (!(File.Exists(RemoteNotifyLogsPath + "\\" + LogName)))
            {
                //log file doesn't exist yet
                return "";
            }
            String LastLine = File.ReadLines(RemoteNotifyLogsPath + "\\" + LogName).Last();
            //Check if there's anything in the log file
            if (LastLine == "TimeGenerated,TimeLogged,TimeCollectedByRemoteNotify,InstanceID,MachineName,Message,SourceApplication,UserResponsible,State,OwningPID,OwningProcessName,ConnectionGuid")
            {
                //No Events have been logged yet
                return "";
            }
            else
            {
                int comma = LastLine.IndexOf(',');
                string TempString = LastLine;
                if (comma != -1)
                {
                    TempString = LastLine.Substring(0, comma);
                }
                return TempString;
            }
        }
    }
}
