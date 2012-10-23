namespace Handsome.Pete.ServiceTemplate
{
    public static class StandardConfig
    {
        static StandardConfig()
        {
            AppSettings = new StandardAppSetting();
            ConnectionStrings = new StandardConnectionString();
        }

        public static StandardAppSetting AppSettings { get; private set; }
        public static StandardConnectionString ConnectionStrings { get; private set; }
    }
}
