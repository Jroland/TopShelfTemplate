using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handsome.Pete.ServiceTemplate;

namespace Handsome.Pete.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = log4net.LogManager.GetLogger("TestHarness");
            var service = new PollingService(log);
            var ep = new ServiceEntryPoint(log, service, args);
            ep.Start();
        }
    }
}
