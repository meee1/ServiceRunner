using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace ServiceRunner
{
    class Service : System.ServiceProcess.ServiceBase
    {
        private System.Diagnostics.EventLog eventLog1;

        protected Thread m_thread;
        protected ManualResetEvent m_shutdownEvent;
        protected TimeSpan m_delay;

        string[] args;

        public Service(string[] args)
        {
            this.args = args;

            this.ServiceName = "ServiceRunner";

            m_delay = new TimeSpan(0, 0, 0, 1, 0);

            
            eventLog1 = new System.Diagnostics.EventLog();
            
            if (!System.Diagnostics.EventLog.SourceExists("ServiceRunner"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "ServiceRunner", "Application");
            }
            eventLog1.Source = "ServiceRunner";
            eventLog1.Log = "Application";
            

            //OnStart(args);
        }

        protected void ServiceMain()
        {
            bool bSignaled = false;
            System.Diagnostics.Process process;

            try
            {

                string[] data = File.ReadAllLines(Application.StartupPath + Path.DirectorySeparatorChar + "cmdline.txt");

                System.Diagnostics.ProcessStartInfo startinfo = new System.Diagnostics.ProcessStartInfo(data[0], data[1]);

                process = System.Diagnostics.Process.Start(startinfo);

            }
            catch (Exception ex) { eventLog1.WriteEntry(ex.ToString()); return; }

            while (true)
            {
                // wait for the event to be signaled
                // or for the configured delay
                bSignaled = m_shutdownEvent.WaitOne(m_delay, true);

                // if we were signaled to shutdow, exit the loop
                if (bSignaled == true)
                    break;

                if (process.HasExited)
                    break;
            }

            process.CloseMainWindow();
            System.Threading.Thread.Sleep(1000);
            process.Kill();

            process.Close();

            eventLog1.WriteEntry("ServiceMain exiting");
        }


        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");

            // create our threadstart object to wrap our delegate method
            ThreadStart ts = new ThreadStart(this.ServiceMain);

            // create the manual reset event and
            // set it to an initial state of unsignaled
            m_shutdownEvent = new ManualResetEvent(false);

            // create the worker thread
            m_thread = new Thread(ts);

            // go ahead and start the worker thread
            m_thread.Start();

            // call the base class so it has a chance
            // to perform any work it needs to
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
            // signal the event to shutdown
            m_shutdownEvent.Set();

            // wait for the thread to stop giving it 10 seconds
            m_thread.Join(10000);

            // call the base class 
            base.OnStop();
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");

            base.OnContinue();
        }

    }
}
