using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Reflection;

namespace N8.Communication
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private readonly Configuration config;
        private string _serviceName;
        private readonly string _company;
        private readonly string _sufix;
        public ProjectInstaller()
        {
            Assembly service = Assembly.GetAssembly(typeof(N8CommunicationService));
            config = ConfigurationManager.OpenExeConfiguration(service.Location);
            _company = (config.AppSettings.Settings["N8.Company"]).Value;

            _sufix = (config.AppSettings.Settings["N8.ServiceSuffix"]).Value;


            _serviceName = $"N8ServiceCommunication{_company}{_sufix}";
			
			InitializeComponent();
        }

        public override void Uninstall(IDictionary savedState)
        {
            _serviceName = $"N8ServiceCommunication{_company}{_sufix}";
            this.serviceInstaller1.DisplayName = _serviceName;
            this.serviceInstaller1.ServiceName = _serviceName;
            base.Uninstall(savedState);
        }

    }
}
