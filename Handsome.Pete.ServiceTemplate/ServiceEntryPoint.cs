using System;
using Topshelf;
using System.Reflection;
using log4net;

namespace Handsome.Pete.ServiceTemplate
{
    /// <summary>
    /// Class that will wrap an IStandardService in a Topshelf hosting environment and provide 
    /// events and methods for controlling the life cycle of a service.
    /// </summary>
    public class ServiceEntryPoint
    {
        #region Events...
        public event Action<ServiceEntryPoint> OnBeforeServiceInitialization;
        public event Action<ServiceEntryPoint> OnBeforeServiceStarted;
        public event Action<ServiceEntryPoint> OnBeforeServiceStopped;
        public event Action OnHelpCommandDisplay;
        #endregion

        #region Private Members...
        private readonly ILog _log;
        private HostControl _serviceController;
        #endregion

        #region Public Properties...
        public IStandardService ServiceReference { get; private set; }
        public string AssemblyFilePath { get; private set; }
        public string WorkingDirectory { get; private set; }
        public StandardAppSetting AppSettings { get { return StandardConfig.AppSettings; } }
        public StandardConnectionString ConnectionStrings { get { return StandardConfig.ConnectionStrings; } }
        public CommandLineParser Parameters { get; private set; }
        #endregion

        #region Constructor...
        /// <summary>
        /// Construct service.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="service"></param>
        /// <param name="args"></param>
        public ServiceEntryPoint(ILog log, IStandardService service, string[] args)
        {
            _log = log;
            AssemblyFilePath = Assembly.GetEntryAssembly() == null ? "" : Assembly.GetEntryAssembly().Location;
            WorkingDirectory = Environment.CurrentDirectory;
            Parameters = new CommandLineParser(args);
            ServiceReference = service;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }
        #endregion

        #region Public Methods...
        /// <summary>
        /// Simulates the start command being sent from the service host.
        /// </summary>
        public void Start()
        {
            try
            {
                // Show version information and working directory
                _log.InfoFormat(Environment.NewLine + Assembly.GetExecutingAssembly().FullName);
                _log.InfoFormat("Assembly Path: {0}", AssemblyFilePath);
                _log.InfoFormat("Current Working Directory: {0}", WorkingDirectory);

                SetServiceName();

                SetAllGenericSettings();

                //throw service init event
                if (OnBeforeServiceInitialization != null)
                    OnBeforeServiceInitialization(this);

                //execute specific commands or default
                if (Parameters.ParamList.ContainsKey("/i"))
                {
                    GetServiceHost(StandardConfig.AppSettings.InstallServiceName, "", "install").Run();
                }
                else if (Parameters.ParamList.ContainsKey("/u"))
                {
                    GetServiceHost(StandardConfig.AppSettings.InstallServiceName, "", "uninstall").Run();
                }
                else if (Parameters.ParamList.ContainsKey("/?"))
                {
                    ShowCommands();
                }
                else
                {
                    GetServiceHost(StandardConfig.AppSettings.InstallServiceName, "", "").Run();
                }
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Exception occured when trying to start the service thread.  Reason:{0}", ex);
                throw;
            }
        }

        /// <summary>
        /// Simulates the stop command being sent from the service host.
        /// </summary>
        public void Stop()
        {
            if (_serviceController != null)
                _serviceController.Stop();
        }

        /// <summary>
        /// Simulates the restart command being sent from the service host.
        /// </summary>
        public void Restart()
        {
            if (_serviceController != null)
                _serviceController.Restart();
        }
        #endregion

        #region Private Methods...
        private void SetAllGenericSettings()
        {
            foreach (var p in Parameters.GetAppSettings())
            {
                StandardConfig.AppSettings.SetAppConfigValue(p.Key, p.Value);
            }

            foreach (var p in Parameters.GetConnectionStrings())
            {
                StandardConfig.ConnectionStrings.SetConnectionConfigValue(p.Key, p.Value);
            }
        }

        private void SetServiceName()
        {
            //look for the partition parameter
            if (Parameters.ParamList.ContainsKey("/s"))
            {
                StandardConfig.AppSettings.InstallServiceName = Parameters.ParamList["/s"];
            }
        }

        private Host GetServiceHost(string serviceName, string description, string commandLine)
        {
            var host = HostFactory.New(config =>
            {
                config.Service<IStandardService>(s =>
                {
                    s.ConstructUsing(name => ServiceReference);
                    s.WhenStarted((ss, control) =>
                    {
                        _serviceController = control;
                        if (OnBeforeServiceStarted != null) OnBeforeServiceStarted(this);
                        return ss.Start();
                    });
                    s.WhenStopped((ss, control) =>
                    {
                        if (OnBeforeServiceStopped != null) OnBeforeServiceStopped(this);
                        return ss.Stop();
                    });
                });
                config.RunAsLocalSystem();

                config.SetDescription(description);
                config.SetDisplayName(serviceName);
                config.SetServiceName(serviceName);
                config.StartAutomatically();
                config.ApplyCommandLine(commandLine);

            });

            return host;
        }

        private void ShowCommands()
        {
            // Show usage
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Usage: " + Assembly.GetExecutingAssembly().ManifestModule + " [command]" + Environment.NewLine + Environment.NewLine + "Where:");
            Console.WriteLine("       No parameters - Will execute as a console app, mimicking a service.");
            Console.WriteLine("       /i - Install service;");
            Console.WriteLine("       /u - Uninstall service.");
            Console.WriteLine("       /s:serviceName - Sets the name of this service.");
            Console.WriteLine("       /a:key|value - Sets an application value to the app settings in the config file.");
            Console.WriteLine("       /c:key|value - Sets a connection value to the app settings in the config file.");
            Console.WriteLine("       /?: - This help message.");

            if (OnHelpCommandDisplay != null)
                OnHelpCommandDisplay();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _log.FatalFormat("Service Template Fatal.  An unhandled exception occured in the service code.  IsTerminating: {0}.  Reason: {1}",
                e.IsTerminating,
                e.ExceptionObject);
        }
        #endregion
    }


}
