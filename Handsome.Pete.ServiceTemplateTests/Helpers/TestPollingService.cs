using System;
using Handsome.Pete.ServiceTemplate;
using log4net;
using System.Threading.Tasks;

namespace Handsome.Pete.ServiceTemplateTests
{
    public class TestPollingService : PollingTemplateEntryPoint
    {
        public TestPollingService(ILog log) : base(log)
        {

        }

        public bool ThrowsPollingException { get; set; }
        public bool ThrowsAppDomainException { get; set; }

        public override void OnServicePolling()
        {
            if (ThrowsPollingException)
                throw new ApplicationException("Uncaught polling error.");

            if (ThrowsAppDomainException)
            {
                var t = new[] { 
                    Task.Factory.StartNew(() => { throw new Exception("Task Exception"); }), 
                    Task.Factory.StartNew(() => { throw new Exception("Task Exception"); }) };

                Task.Factory.ContinueWhenAll(t, x => { }).Wait();
            }
        }

        public override void OnServiceDispose(Task mainThread)
        {
           
        }

        public override void OnServiceInterrupted()
        {
            
        }
    }
}
