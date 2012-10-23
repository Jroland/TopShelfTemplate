using System;
using System.Threading.Tasks;
using System.Threading;
using log4net;

namespace Handsome.Pete.ServiceTemplate
{
    public abstract class PollingTemplateEntryPoint : IPollingService
    {
        #region Protected Members...
        protected readonly ILog Log;
        #endregion

        #region Public Properties...
        public bool Interrupted { get; set; }
        public Task PollingTask { get; private set; }
        public Task DisposingTask { get; private set; }
        #endregion

        protected PollingTemplateEntryPoint(ILog log)
        {
            Log = log;
        }

        public bool Start()
        {
            try
            {
                PollingTask = new Task(() =>
                {
                    try
                    {
                        Log.InfoFormat("{0} has started.", StandardConfig.AppSettings.InstallServiceName);

                        while (Interrupted == false)
                        {
                            OnServicePolling();

                            SleepService(StandardConfig.AppSettings.ServicePollIntervalSeconds);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("Exception occurred while attempting to poll.  Reason: {0}", ex);
                    }
                }, TaskCreationOptions.LongRunning);

                DisposingTask = PollingTask.ContinueWith(OnServiceDispose);               
                PollingTask.Start();
                return true;
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Failed to initialize the main thread.  Reason: {0}", ex.Message);
                throw;
            }
        }

        public bool Stop()
        {
            Log.DebugFormat("PollingTemplateEntryPoint Stop methods called.");
            Interrupted = true;

            try
            {
                OnServiceInterrupted();

                Task.WaitAll(new[] { PollingTask, DisposingTask });

                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error occured while trying to unwind the service.  Error: {0}", ex.Message);
                return false;
            }
        }

        #region Abstract Methods...
        public abstract void OnServicePolling();
        public abstract void OnServiceDispose(Task pollingTask);
        public abstract void OnServiceInterrupted();
        #endregion

        #region Private Methods...
        private void SleepService(int delaySeconds)
        {
            var delayExpire = DateTime.Now.AddSeconds(delaySeconds).Ticks;

            while (Interrupted == false && DateTime.Now.Ticks < delayExpire)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
        #endregion
    }
}
