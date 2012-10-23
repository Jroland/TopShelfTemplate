using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handsome.Pete.ServiceTemplate;
using NUnit.Framework;
using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.Moq;
using log4net;
using Moq;


namespace Handsome.Pete.ServiceTemplateTests
{
    [TestFixture]
    public class ServiceEntryPointTests
    {
        private readonly MoqMockingKernel _kernel = new MoqMockingKernel();

        [Test]
        public void ServiceEntryPointUsesNamespaceAsServiceNameByDefault()
        {
            var service = _kernel.Get<TestPollingService>();
            _kernel.Bind<IStandardService>().ToConstant(service);
            var ep = _kernel.Get<ServiceEntryPoint>();
            Assert.AreEqual("Handsome.Pete.ServiceTemplateTests", ep.AppSettings.InstallServiceName);
        }

        //[Test]
        //public void AppDomainUnhandledExceptionLogged()
        //{
        //    var log = Kernel.GetMock<ILog>();

        //    var service = Kernel.Get<TestPollingService>();
        //    Kernel.Bind<IStandardService>().ToConstant(service);

        //    var ep = kernel.Get<ServiceEntryPoint>(); 
        //    ep.StartServiceThread();

        //    log.Verify(x => x.FatalFormat(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        //}
    }
}
