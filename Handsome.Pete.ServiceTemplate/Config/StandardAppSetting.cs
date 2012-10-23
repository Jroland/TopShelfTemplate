using System;
using System.Configuration;
using System.IO;

namespace Handsome.Pete.ServiceTemplate
{
    public class StandardAppSetting
    {
        private const int STANDARD_POLLING_SECONDS_DEFAULT = 60;
        private readonly Lazy<string> _serivceWorkingDirectory = new Lazy<string>(() => Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));

        public string InstallServiceName
        {
            get { return GetAppSettingDefault("InstallServiceName", System.Reflection.Assembly.GetCallingAssembly().GetName().Name); }
            set { SetAppConfigValue("InstallServiceName", value); }
        }

        public string UniqueServiceId
        {
            get { return GetAppSettingDefault("UniqueServiceId", string.Concat(Environment.MachineName, ".", InstallServiceName)); }
        }

        public int ServicePollIntervalSeconds
        {
            get
            {
                return Int32.Parse(GetAppSettingDefault("ServicePollIntervalSeconds", STANDARD_POLLING_SECONDS_DEFAULT.ToString()));
            }
            set { SetAppConfigValue("ServicePollIntervalSeconds", value.ToString()); }
        }

        public string SerivceWorkingDirectory
        {
            get { return _serivceWorkingDirectory.Value; }
        }

        public void SetAppConfigValue(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        protected string GetAppSettingDefault(string key, string defaultValue)
        {
            if (ConfigurationManager.AppSettings[key] == null)
                SetAppConfigValue(key, defaultValue);

            return ConfigurationManager.AppSettings[key];
        }
    }
}
