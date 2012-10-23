using System.Threading.Tasks;

namespace Handsome.Pete.ServiceTemplate
{
    public interface IPollingService : IStandardService
    {
        void OnServicePolling();
        void OnServiceDispose(Task mainThread);
        void OnServiceInterrupted();
    }
}
