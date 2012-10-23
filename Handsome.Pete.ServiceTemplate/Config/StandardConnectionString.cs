using System.Configuration;

namespace Handsome.Pete.ServiceTemplate
{
    public class StandardConnectionString
    {
        public void SetConnectionConfigValue(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.ConnectionStrings.ConnectionStrings[key] == null)
                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(key, value));
            else
                config.ConnectionStrings.ConnectionStrings[key].ConnectionString = value;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}
