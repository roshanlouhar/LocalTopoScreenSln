using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace TPService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
       
        //private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        //{
        //    //The following code starts the services after it is installed.
        //    using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController(serviceInstaller1.ServiceName))
        //    {
        //        serviceController.Start();
        //    }
        //}

        //private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        //{
        //    //The following code starts the services after it is installed.
        //    using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController(serviceInstaller1.ServiceName))
        //    {
        //        serviceController.Start();
        //    }
        //}
    }
}
