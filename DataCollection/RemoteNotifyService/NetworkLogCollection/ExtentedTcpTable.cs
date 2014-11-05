using System;
using System.Runtime.InteropServices;

namespace RemoteNotify
{
    class ExtentedTcpTable
    {
        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, uint reserved = 0);

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            // DWORD is System.UInt32 in C#
            public System.UInt32 state;
            public System.UInt32 localAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] localPort;
            public System.UInt32 remoteAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] remotePort;
            public System.UInt32 owningPid;

            public System.Net.IPAddress LocalAddress
            {
                get
                {
                    return new System.Net.IPAddress(localAddr);
                }
            }

            public ushort LocalPort
            {
                get
                {
                    return BitConverter.ToUInt16(
                    new byte[2] { localPort[1], localPort[0] }, 0);
                }
            }

            public System.Net.IPAddress RemoteAddress
            {
                get
                {
                    return new System.Net.IPAddress(remoteAddr);
                }
            }

            public ushort RemotePort
            {
                get
                {
                    return BitConverter.ToUInt16(
                    new byte[2] { remotePort[1], remotePort[0] }, 0);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            MIB_TCPROW_OWNER_PID table;
        }

        enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }

        public struct ConnectionWithGuid
        {
            public MIB_TCPROW_OWNER_PID Connection;
            public Guid ConnectionGuid;
        }

        public MIB_TCPROW_OWNER_PID[] GetAllTcpConnections()
        {
            // TcpRow is my own class to display returned rows in a nice manner.
            MIB_TCPROW_OWNER_PID[] tTable;
            int AF_INET = 2;    // IP_v4
            int buffSize = 0;

            // how much memory do we need?
            uint ret = GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);

            try
            {
                ret = GetExtendedTcpTable(buffTable, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
                if (ret != 0)
                {
                    return null;
                }

                // get the number of entries in the table
                MIB_TCPTABLE_OWNER_PID tab = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));
                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                // buffer we will be returning;
                tTable = new MIB_TCPROW_OWNER_PID[tab.dwNumEntries];
       
                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    MIB_TCPROW_OWNER_PID tcpRow = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));
                    tTable[i] = tcpRow;
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(tcpRow));   // next entry
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable;
        }

        public ConnectionWithGuid[] AssignGuidToEachConnection(MIB_TCPROW_OWNER_PID[] Connections)
        {
            ConnectionWithGuid[] TempConnectionsStruct = new ConnectionWithGuid[Connections.Length];
            for (int i = 0; i < Connections.Length; i++)
            {
                TempConnectionsStruct[i].Connection = Connections[i];
                TempConnectionsStruct[i].ConnectionGuid = Guid.NewGuid();
            }
            return TempConnectionsStruct;
        }

    }
}
