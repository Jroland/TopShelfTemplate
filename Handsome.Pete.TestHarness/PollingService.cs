using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handsome.Pete.ServiceTemplate;
using log4net;

namespace Handsome.Pete.TestHarness
{
    public class PollingService : PollingTemplateEntryPoint
    {
        public PollingService(ILog log) : base(log)
        {

        }

        public override void OnServicePolling()
        {
            Console.WriteLine("Ping.");
        }

        public override void OnServiceDispose(System.Threading.Tasks.Task pollingTask)
        {
            Console.WriteLine("Disposing.");
        }

        public override void OnServiceInterrupted()
        {
            Console.WriteLine("Interrupted.");
        }
    }
}
