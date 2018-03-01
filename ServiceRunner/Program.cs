using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.ServiceProcess;

namespace ServiceRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var serv = new Service(args);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { 
                serv
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
