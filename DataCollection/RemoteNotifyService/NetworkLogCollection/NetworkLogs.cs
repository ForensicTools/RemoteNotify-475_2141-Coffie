using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RemoteNotify
{
    class NetworkLogs
    {
        private String RemoteNotifyLogsPath = "C:\\RemoteNotify\\Logs";
        private String LogName = "NetworkConnectionsLog.csv";
        private RemoteNotify.ExtentedTcpTable.ConnectionWithGuid[] PreviousConnectionsWithGuid = null;

        //Collects active connections to the computer
        public void CollectNetworkLogs()
        {
            //Check if log path is created
            if (!(Directory.Exists(RemoteNotifyLogsPath)))
            {
                Directory.CreateDirectory(RemoteNotifyLogsPath);
            }

            //Check if network log is created
            if (!(File.Exists(RemoteNotifyLogsPath + "\\" + LogName)))
            {
                //create CSV File and add Header column names
                using (StreamWriter writer = File.CreateText(RemoteNotifyLogsPath + "\\" + LogName))
                {
                    writer.WriteLine("Time,NEW-CHANGED,LocalAddress,LocalPort,RemoteAddress,RemotePort,State,OwningPID,OwningProcessName,ConnectionGuid");
                }
            }

            while (true)
            {
                try
                {
                    //Have it run every 5 seconds
                    Thread.Sleep(1000);
                    //Need to log all TCP connections
                    //IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
                    //TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

                    //New way of getting TCP connections that maps the connections to a PID
                    ExtentedTcpTable TcpConnectionsObject = new ExtentedTcpTable();
                    RemoteNotify.ExtentedTcpTable.MIB_TCPROW_OWNER_PID[] connections = TcpConnectionsObject.GetAllTcpConnections();
                    RemoteNotify.ExtentedTcpTable.ConnectionWithGuid[] ConnectionsWithGuid = TcpConnectionsObject.AssignGuidToEachConnection(connections);

                    //Check for new conections
                    foreach (RemoteNotify.ExtentedTcpTable.ConnectionWithGuid c in ConnectionsWithGuid)
                    {
                        bool repeated = false;
                        //Check if there is a list of previous connections
                        if (PreviousConnectionsWithGuid != null)
                        {
                            //interate to see if the connection is in the previous connection log
                            foreach (RemoteNotify.ExtentedTcpTable.ConnectionWithGuid pc in PreviousConnectionsWithGuid)
                            {
                                if (CompareTwoConnections(c, pc))
                                {
                                    repeated = true;
                                    break;
                                }
                            }
                            if (repeated)
                            {
                                continue;
                            }
                            else
                            {
                                //Log as new connection
                                WriteToNetworkLog(c, "NEW");
                            }
                        }
                        else
                        {
                            //Log connection (c) because previousconnections is null
                            WriteToNetworkLog(c, "NEW");
                        }
                    }


                    //check for ended connections
                    if (PreviousConnectionsWithGuid != null)
                    {
                        foreach (RemoteNotify.ExtentedTcpTable.ConnectionWithGuid pc in PreviousConnectionsWithGuid)
                        {
                            bool ConnectionClosed = true;
                            foreach (RemoteNotify.ExtentedTcpTable.ConnectionWithGuid c in ConnectionsWithGuid)
                            {
                                if (CompareTwoConnections(c, pc))
                                {
                                    ConnectionClosed = false;
                                    break;
                                }
                            }
                            if (ConnectionClosed)
                            {
                                WriteToNetworkLog(pc, "CHANGED");
                            }
                        }
                    }

                    //copy current connections to previous connections
                    PreviousConnectionsWithGuid = ConnectionsWithGuid;

                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
            }
        }

        //Writes to Network log
        //if file is open in another program it will open a new hidden log and write the contents there
        //When the main log is closed they will be merged back
        private void WriteToNetworkLog(RemoteNotify.ExtentedTcpTable.ConnectionWithGuid Connection, String ConnectionType)
        {
            
            try
            {
                //Check if there's temp network log and merge if there is
                MergeTempNetworkLog();

                //Write to log
                using (StreamWriter writer = new StreamWriter(RemoteNotifyLogsPath + "\\" + LogName, true))
                {
                    if (!(Connection.Connection.LocalAddress.ToString().Contains("[::1]")) &&
                        !(Connection.Connection.LocalAddress.ToString().Contains("127.0.0.1")) &&
                        !(Connection.Connection.RemoteAddress.ToString().Contains("0.0.0.0")))
                    {
                        try
                        {
                            writer.WriteLine(DateTime.Now.ToString() + "," +
                                ConnectionType + "," +
                                Connection.Connection.LocalAddress.ToString() + "," +
                                Connection.Connection.LocalPort.ToString() + "," +
                                Connection.Connection.RemoteAddress.ToString() + "," +
                                Connection.Connection.RemotePort.ToString() + "," +
                                Connection.Connection.state + "," +
                                Connection.Connection.owningPid + "," +
                                Process.GetProcessById(checked((int)Connection.Connection.owningPid)).ProcessName + "," + 
                                Connection.ConnectionGuid.ToString()
                                );
                        }
                        catch(ArgumentException)
                        {
                            writer.WriteLine(DateTime.Now.ToString() + "," +
                                ConnectionType + "," +
                                Connection.Connection.LocalAddress.ToString() + "," +
                                Connection.Connection.LocalPort.ToString() + "," +
                                Connection.Connection.RemoteAddress.ToString() + "," +
                                Connection.Connection.RemotePort.ToString() + "," +
                                Connection.Connection.state + "," +
                                Connection.Connection.owningPid + "," +
                                "UNKNOWN" + "," +
                                Connection.ConnectionGuid.ToString()
                            );
                        }
                        catch(OverflowException)
                        {
                            writer.WriteLine(DateTime.Now.ToString() + "," +
                                ConnectionType + "," +
                                Connection.Connection.LocalAddress.ToString() + "," +
                                Connection.Connection.LocalPort.ToString() + "," +
                                Connection.Connection.RemoteAddress.ToString() + "," +
                                Connection.Connection.RemotePort.ToString() + "," +
                                Connection.Connection.state + "," +
                                Connection.Connection.owningPid + "," +
                                "UNKNOWN" + "," +
                                Connection.ConnectionGuid.ToString()
                                );
                        }
                    }
                }
            }
            catch (IOException)
            {
                //Check if temporary network log is created
                if (!(File.Exists(RemoteNotifyLogsPath + "\\" + LogName + ".tmp")))
                {
                    if (!(Connection.Connection.LocalAddress.ToString().Contains("[::1]")) &&
                        !(Connection.Connection.LocalAddress.ToString().Contains("127.0.0.1")) &&
                        !(Connection.Connection.RemoteAddress.ToString().Contains("0.0.0.0")))
                    {
                        //create CSV File and add log content
                        using (StreamWriter writer = File.CreateText(RemoteNotifyLogsPath + "\\" + LogName + ".tmp"))
                        {
                            try
                            {
                                writer.WriteLine(DateTime.Now.ToString() + "," +
                                    ConnectionType + "," +
                                    Connection.Connection.LocalAddress.ToString() + "," +
                                    Connection.Connection.LocalPort.ToString() + "," +
                                    Connection.Connection.RemoteAddress.ToString() + "," +
                                    Connection.Connection.RemotePort.ToString() + "," +
                                    Connection.Connection.state + "," +
                                    Connection.Connection.owningPid + "," +
                                    Process.GetProcessById(checked((int)Connection.Connection.owningPid)).ProcessName + "," +
                                    Connection.ConnectionGuid.ToString()
                                    );
                            }
                            catch (ArgumentException)
                            {
                                writer.WriteLine(DateTime.Now.ToString() + "," +
                                    ConnectionType + "," +
                                    Connection.Connection.LocalAddress.ToString() + "," +
                                    Connection.Connection.LocalPort.ToString() + "," +
                                    Connection.Connection.RemoteAddress.ToString() + "," +
                                    Connection.Connection.RemotePort.ToString() + "," +
                                    Connection.Connection.state + "," +
                                    Connection.Connection.owningPid + "," +
                                    "UNKNOWN" + "," +
                                    Connection.ConnectionGuid.ToString()
                                    );
                            }
                            catch (OverflowException)
                            {
                                writer.WriteLine(DateTime.Now.ToString() + "," +
                                    ConnectionType + "," +
                                    Connection.Connection.LocalAddress.ToString() + "," +
                                    Connection.Connection.LocalPort.ToString() + "," +
                                    Connection.Connection.RemoteAddress.ToString() + "," +
                                    Connection.Connection.RemotePort.ToString() + "," +
                                    Connection.Connection.state + "," +
                                    Connection.Connection.owningPid + "," +
                                    "UNKNOWN" + "," +
                                    Connection.ConnectionGuid.ToString()
                                    );
                            }
                        }
                    }
                    try
                    {
                        File.SetAttributes(RemoteNotifyLogsPath + "\\" + LogName + ".tmp", File.GetAttributes(RemoteNotifyLogsPath + "\\" + LogName + ".tmp") | FileAttributes.Hidden);
                    }
                    catch (FileNotFoundException)
                    {
                        return;
                    }
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(RemoteNotifyLogsPath + "\\" + LogName + ".tmp", true))
                    {
                        if (!(Connection.Connection.LocalAddress.ToString().Contains("[::1]")) &&
                        !(Connection.Connection.LocalAddress.ToString().Contains("127.0.0.1")) &&
                        !(Connection.Connection.RemoteAddress.ToString().Contains("0.0.0.0")))
                        {
                            try
                            {
                                writer.WriteLine(DateTime.Now.ToString() + "," +
                                    ConnectionType + "," +
                                    Connection.Connection.LocalAddress.ToString() + "," +
                                    Connection.Connection.LocalPort.ToString() + "," +
                                    Connection.Connection.RemoteAddress.ToString() + "," +
                                    Connection.Connection.RemotePort.ToString() + "," +
                                    Connection.Connection.state + "," +
                                    Connection.Connection.owningPid + "," +
                                    Process.GetProcessById(checked((int)Connection.Connection.owningPid)).ProcessName + "," +
                                    Connection.ConnectionGuid.ToString()
                                    );
                            }
                            catch (ArgumentException)
                            {
                                writer.WriteLine(DateTime.Now.ToString() + "," +
                                    ConnectionType + "," +
                                    Connection.Connection.LocalAddress.ToString() + "," +
                                    Connection.Connection.LocalPort.ToString() + "," +
                                    Connection.Connection.RemoteAddress.ToString() + "," +
                                    Connection.Connection.RemotePort.ToString() + "," +
                                    Connection.Connection.state + "," +
                                    Connection.Connection.owningPid + "," +
                                    "UNKNOWN" + "," +
                                    Connection.ConnectionGuid.ToString()
                                    );
                            }
                            catch (OverflowException)
                            {
                                writer.WriteLine(DateTime.Now.ToString() + "," +
                                    ConnectionType + "," +
                                    Connection.Connection.LocalAddress.ToString() + "," +
                                    Connection.Connection.LocalPort.ToString() + "," +
                                    Connection.Connection.RemoteAddress.ToString() + "," +
                                    Connection.Connection.RemotePort.ToString() + "," +
                                    Connection.Connection.state + "," +
                                    Connection.Connection.owningPid + "," +
                                    "UNKNOWN" + "," +
                                    Connection.ConnectionGuid.ToString()
                                );
                            }
                        }
                    }
                }
            }
        }

        //Compare two connections
        //Returns true if they are the same
        private bool CompareTwoConnections(RemoteNotify.ExtentedTcpTable.ConnectionWithGuid c, RemoteNotify.ExtentedTcpTable.ConnectionWithGuid pc)
        {
            if (c.Connection.LocalAddress.ToString() == pc.Connection.LocalAddress.ToString() &&
                c.Connection.LocalPort.ToString() == pc.Connection.LocalPort.ToString() &&
                c.Connection.RemoteAddress.ToString() == pc.Connection.RemoteAddress.ToString() &&
                c.Connection.RemotePort.ToString() == pc.Connection.RemotePort.ToString() &&
                c.Connection.owningPid.ToString() == pc.Connection.owningPid.ToString())
                return true;
            else
                return false;
        }

        //Check if there are temporary logs and Merge if there are
        private void MergeTempNetworkLog()
        {
            if (File.Exists(RemoteNotifyLogsPath + "\\" + LogName + ".tmp"))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(RemoteNotifyLogsPath + "\\" + LogName + ".tmp"))
                    {
                        String TempLogEntries = reader.ReadToEnd();
                        using (StreamWriter writer = new StreamWriter(RemoteNotifyLogsPath + "\\" + LogName, true))
                        {
                            writer.Write(TempLogEntries);
                        }
                    }
                    File.Delete(RemoteNotifyLogsPath + "\\" + LogName + ".tmp");
                }
                catch
                {
                    return;
                }
            }
        }

    }
}
