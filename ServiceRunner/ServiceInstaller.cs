using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace ServiceRunner
{
    [RunInstaller(true)]
    public class ServiceRunnerInstaller : Installer
    {
        public ServiceRunnerInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.NetworkService;

            serviceInstaller.DisplayName = "Service Runner";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "ServiceRunner";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
