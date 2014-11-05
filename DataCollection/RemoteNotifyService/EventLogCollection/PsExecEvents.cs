using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteNotify.EventLogCollection
{
    class PsExecEvents
    {
        public void CollectPsExecEventsFromSystem()
        {
            while (true)
            {
                try
                {
                    //Have it run every second
                    Thread.Sleep(1000);

                    //Do stuff here

                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
            }
        }
        public void CollectPsExecEventsFromSecurity()
        {
            while (true)
            {
                try
                {
                    //Have it run every second
                    Thread.Sleep(1000);

                    //Do stuff here

                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
            }
        }
    }
}
