using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteNotify
{
    [RunInstaller(true)]
    public partial class RemoteNotifyServiceInstaller : System.Configuration.Install.Installer
    {
        public RemoteNotifyServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
